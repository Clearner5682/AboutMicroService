using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AspectCore;
using AspectCore.DynamicProxy;
using Microsoft.Extensions.Caching.Memory;
using Polly;

namespace WebApp1
{
    [AttributeUsage(AttributeTargets.Method)]
    public class HystrixCommandAttribute : AbstractInterceptorAttribute
    {
        /// <summary>
        /// 服务降级的处理方法
        /// </summary>
        public string FallBackMethod { get; set; }

        /// <summary>
        /// 最多重试几次
        /// </summary>
        public int MaxRetryTimes { get; set; } = 0;

        /// <summary>
        /// 重试间隔秒数
        /// </summary>
        public int RetryIntervalMilliseconds { get; set; } = 100;

        /// <summary>
        /// 是否启用断路器
        /// </summary>
        public bool IsEnableCircuitBreaker { get; set; } = false;


        /// <summary>
        /// 出现几次错误就熔断
        /// </summary>
        public int ExceptionsAllowedBeforeBreaking { get; set; } = 1;

        /// <summary>
        /// 每次熔断多少毫秒
        /// </summary>
        public int MillisecondsOfBreak { get; set; } = 3000;

        /// <summary>
        /// 执行超过多少毫秒就认为超时(0表示不检测超时)
        /// </summary>
        public int TimeOutMilliseconds { get; set; } = 0;

        /// <summary>
        /// 缓存多少毫秒（0表示不缓存），用“类名+方法名+所有参数ToString拼接”做缓存Key
        /// </summary>
        public int CacheTTLMilliseconds { get; set; } = 0;

        private static ConcurrentDictionary<MethodInfo, IAsyncPolicy> policies = new ConcurrentDictionary<MethodInfo, IAsyncPolicy>();

        private static readonly IMemoryCache memoryCache = new MemoryCache(new Microsoft.Extensions.Caching.Memory.MemoryCacheOptions());

        public HystrixCommandAttribute(string fallBack)
        {
            this.FallBackMethod = fallBack;
        }

        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            policies.TryGetValue(context.ServiceMethod, out IAsyncPolicy policy);
            if (policy == null)
            {
                var policyBuilder = Policy.Handle<Exception>();
                var fallback = policyBuilder.FallbackAsync(async (context,cancelToken) => {
                    await Task.Run(()=> {
                        AspectContext aspectContext = (AspectContext)context["aspectContext"];
                        var fallBackMethod = aspectContext.ServiceMethod.DeclaringType.GetMethod(this.FallBackMethod);
                        Object fallBackResult = fallBackMethod.Invoke(aspectContext.Implementation, aspectContext.Parameters);
                        //不能如下这样，因为这是闭包相关，如果这样写第二次调用Invoke的时候context指向的
                        //还是第一次的对象，所以要通过Polly的上下文来传递AspectContext
                        //context.ReturnValue = fallBackResult;
                        aspectContext.ReturnValue = fallBackResult;
                    });
                },
                onFallbackAsync: (ex,context)=> {
                    Console.WriteLine($"执行服务降级的替代方法");
                    return Task.CompletedTask;
                });
                if (IsEnableCircuitBreaker)
                {
                    var circuitBreaker=policyBuilder.CircuitBreakerAsync(ExceptionsAllowedBeforeBreaking, TimeSpan.FromMilliseconds(MillisecondsOfBreak),
                        onBreak:(ex,timespan)=> {
                            Console.WriteLine($"断路器已打开，异常：{ex.Message}");
                        },
                        onReset:()=> {
                            Console.WriteLine("断路器已关闭");
                        },
                        onHalfOpen:()=> {
                            Console.WriteLine("熔断时间到了，断路器进入半开启状态");
                        });
                    policy = Policy.WrapAsync(fallback, circuitBreaker);
                }
                if (MaxRetryTimes > 0)
                {
                    var retry = policyBuilder.RetryAsync(MaxRetryTimes,onRetry:(ex,index)=> {
                        Console.WriteLine($"第{index}次重试");
                    });
                    policy = Policy.WrapAsync(policy, retry);
                }
                if (TimeOutMilliseconds > 0)
                {
                    var timeOut = Policy.TimeoutAsync(TimeSpan.FromMilliseconds(TimeOutMilliseconds), Polly.Timeout.TimeoutStrategy.Pessimistic, onTimeoutAsync: (context, timespan, task) => {
                        Console.WriteLine("业务方法超时了");
                        return Task.CompletedTask;
                    });
                    policy = Policy.WrapAsync(policy, timeOut);
                }
                
                //放入
                policies.TryAdd(context.ServiceMethod, policy);
            }


            //把本地调用的AspectContext传递给Polly，主要给FallbackAsync中使用，避免闭包的坑
            Context pollyCtx = new Context();
            pollyCtx["aspectContext"] = context;

            //Install-Package Microsoft.Extensions.Caching.Memory
            if (CacheTTLMilliseconds > 0)
            {
                //用类名+方法名+参数的下划线连接起来作为缓存key
                string cacheKey = "HystrixMethodCacheManager_Key_" + context.ServiceMethod.DeclaringType
                                                                   + "." + context.ServiceMethod + string.Join("_", context.Parameters);
                //尝试去缓存中获取。如果找到了，则直接用缓存中的值做返回值
                if (memoryCache.TryGetValue(cacheKey, out var cacheValue))
                {
                    context.ReturnValue = cacheValue;
                }
                else
                {
                    //如果缓存中没有，则执行实际被拦截的方法
                    await policy.ExecuteAsync(ctx => next(context), pollyCtx);
                    //存入缓存中
                    using (var cacheEntry = memoryCache.CreateEntry(cacheKey))
                    {
                        cacheEntry.Value = context.ReturnValue;
                        cacheEntry.AbsoluteExpiration = DateTime.Now + TimeSpan.FromMilliseconds(CacheTTLMilliseconds);
                    }
                }
            }
            else//如果没有启用缓存，就直接执行业务方法
            {
                await policy.ExecuteAsync(ctx => next(context), pollyCtx);
            }
        }
    }
}

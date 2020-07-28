using System;
using System.Net.Http;
using System.Threading;
using Polly;
using Polly.Timeout;

namespace PollyDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            // 这里必须执行超时策略为悲观，才会抛出超时异常
            Policy.Timeout(TimeSpan.FromSeconds(1),timeoutStrategy:TimeoutStrategy.Pessimistic, onTimeout: (context, timespan, task) => {
                Console.WriteLine($"超时了，超时时间：{timespan.TotalSeconds}");
            }).Execute(()=> {
                Console.WriteLine("开始执行业务");
                Thread.Sleep(5000);
                Console.WriteLine("执行业务结束");
            });


            Console.ReadKey();
        }

        public static void Test()
        {
            Policy.Handle<Exception>()
                .Fallback(() => {
                    Console.WriteLine("这就是服务降级的处理方法");
                }, ex => {
                    Console.WriteLine(ex.Message);
                })
                .Execute(() => {
                    Console.WriteLine("调用其他服务，执行业务逻辑");

                    throw new ArgumentException("异常发生了");
                });

            // 带条件的异常
            Policy.Handle<Exception>(ex => ex.Message == "Your Error");
            // 处理多种异常
            Policy.Handle<ArgumentNullException>()
                .Or<HttpRequestException>()
                .Or<NullReferenceException>();
            // Polly用来处理不可控的异常，比如服务A请求服务B，由于网络原因出错了
            // try catch用来处理明确的异常


            // 弹性策略，响应性策略，重试、断路器


            // 重试
            Policy.Handle<Exception>().Retry();// 重试1次
            Policy.Handle<Exception>().Retry(3);// 重试3次
            Policy.Handle<Exception>().Retry(retryCount: 3, onRetry: (ex, index) => { });// 带重试回调函数
            Policy.Handle<Exception>().RetryForever();// 一直重试，直到成功
            Policy.Handle<Exception>().WaitAndRetryAsync(3, (index) => {
                return TimeSpan.FromSeconds(Math.Pow(2, index));// 每次重试等待市场为2的N次方
            });// 重试且等待

            // 普通断路器，非常实用的策略
            // 连续3次触发指定的故障后，就开启断路器（OPEN），进入熔断状态1分钟
            var circuitBreakerPolicy = Policy.Handle<Exception>()
                .CircuitBreaker(3, TimeSpan.FromMinutes(1),
                onBreak: (ex, timespan) => { },// 开启断路器OPEN 
                onReset: () => { });// 恢复，关闭断路器CLOSED
            circuitBreakerPolicy.Isolate();// 手动开启断路器，状态为ISOLATED
            circuitBreakerPolicy.Reset();// 手动关闭断路器，状态为CLOSED

            // 高级断路器
            // 在采样时间10秒内，至少执行了8次采样，失败率超过阈值（0.5），则熔断30秒，进入OPEN状态
            // 30秒过了，会变成HALF-OPEN，半开启状态，断路器会尝试释放1次操作，尝试去请求
            // 如果成功，则变成CLOSED，如果失败，则断路器打开（熔断30秒）进入OPEN状态
            Policy.Handle<Exception>()
                .AdvancedCircuitBreaker(
                0.5, // 失败的阈值
                TimeSpan.FromSeconds(10), // 采样时间10秒 
                8, // 采样时间内至少有8次请求
                TimeSpan.FromSeconds(30)); // 熔断30秒

            // 主动式的策略，不需要定义故障
            // 超时
            // 在3秒内还未调用完服务，则会自动抛出一个TimeoutRejectedException类型的异常
            Policy.Timeout(3, (context, timespan, arg3, arg4) => { });

            // 舱壁隔离，控制并发数量
            // 最多12个并发，20个等待，其他的全部拒绝
            Policy.Bulkhead(maxParallelization: 12, maxQueuingActions: 20, onBulkheadRejected: context => { });


            // 策略包装，策略组合
            // 顺序是从右至左
            var policyBuilder = Policy.Handle<Exception>();
            var fallbackPolicy = policyBuilder.Fallback(() => {
                Console.WriteLine("执行服务降级的操作");
            });
            var retryPolicy = policyBuilder.Retry(3, (ex, index) => {
                Console.WriteLine($"重试第{index}次，故障信息：{ex.Message}");
            });
            // 组合策略，从右到左
            Policy.Wrap(fallbackPolicy, retryPolicy).Execute(() => {
                Console.WriteLine("执行服务调用");

                throw new HttpRequestException("网络出错了");
            });

            #region 多个策略的组合
            //var policyBuilder = Policy.Handle<Exception>();
            //var fallbackPolicy = policyBuilder.Fallback(() => {
            //    // 返回替代数据
            //    Console.WriteLine("服务降级，返回替代数据");
            //});
            //var timeoutPolicy = Policy.Timeout(
            //    1, // 超过1秒就超时
            //    onTimeout: (context, timespan, task) => {
            //        Console.WriteLine($"超时了，执行秒数：{timespan.TotalSeconds}");
            //    }); // 超时时执行的事件
            //// 执行2次重试
            //var retryPolicy = policyBuilder.Retry(2, onRetry: (ex, index) => {
            //    Console.WriteLine($"第{index}次重试,故障信息：{ex.Message}");
            //});
            //var circuitPolicy = policyBuilder.CircuitBreaker(
            //    2, // 失败2次才熔断
            //    TimeSpan.FromSeconds(5), // 每次熔断5分钟 
            //    onBreak: (ex, timespan) => {
            //        Console.WriteLine("熔断开启");
            //    }, // 熔断开启时执行
            //    onReset: () => {
            //        Console.WriteLine("熔断关闭");
            //    }, // 熔断关闭时执行
            //    onHalfOpen: () => {
            //        Console.WriteLine("熔断时间到了，进入半开启状态");
            //    }); // 熔断时间到了，进入半开启状态

            //var policyWrapper = Policy.Wrap(fallbackPolicy, circuitPolicy, retryPolicy, timeoutPolicy);
            #endregion
        }
    }
}

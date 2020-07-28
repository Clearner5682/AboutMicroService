using Polly;
using System;
using System.Threading.Tasks;

namespace PollyHelper
{
    public static class PollyHelper
    {
        public static ISyncPolicy CreatePolicy()
        {
            var policyBuilder = Policy.Handle<Exception>();
            var fallbackPolicy = policyBuilder.Fallback(() => {
                // 返回替代数据
                Console.WriteLine("服务降级，返回替代数据");
            });
            var timeoutPolicy = Policy.Timeout(
                TimeSpan.FromSeconds(2), // 超过2秒就超时
                Polly.Timeout.TimeoutStrategy.Pessimistic, // 指定超时策略为悲观，才会抛出超时异常，后续的策略才会执行
                onTimeout: (context, timespan, task) =>
                {
                    Console.WriteLine($"超时了，执行秒数：{timespan.TotalSeconds}");
                } // 超时时执行的事件
                ); 
            // 执行2次重试
            var retryPolicy = policyBuilder.Retry(2, onRetry: (ex, index) => {
                Console.WriteLine($"第{index}次重试,故障信息：{ex.Message}");
            });
            var circuitPolicy = policyBuilder.CircuitBreaker(
                2, // 失败2次才熔断
                TimeSpan.FromSeconds(5), // 每次熔断5秒钟 
                onBreak: (ex, timespan) => {
                    Console.WriteLine("熔断开启");
                }, // 熔断开启时执行
                onReset: () => {
                    Console.WriteLine("熔断关闭");
                }, // 熔断关闭时执行
                onHalfOpen: () => {
                    Console.WriteLine("熔断时间到了，进入半开启状态");
                }); // 熔断时间到了，进入半开启状态

            var policyWrapper = Policy.Wrap(fallbackPolicy, circuitPolicy, retryPolicy, timeoutPolicy);

            return policyWrapper;
        }
    }
}

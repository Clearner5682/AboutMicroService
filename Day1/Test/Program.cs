using ServiceDiscovery;
using ServiceDiscovery.LoadBalancer;
using System;
using System.Net.Http;
using PollyHelper;
using System.Threading;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            // 负载均衡的几种算法
            // 随机
            // 轮询
            // 加权轮询
            // 动态轮询，根据各个服务的请求次数动态计算权重
            // 最小活跃连接
            // 一致性hash
            var serviceProvider = new ConsulServiceProvider("http://127.0.0.1:8500");
            var services = serviceProvider.GetServicesAsync("WebApp1").Result;
            foreach(var service in services)
            {
                Console.WriteLine(service);
            }

            var httpClient = new HttpClient();

            // 使用负载均衡来获取一个服务
            var policy = PollyHelper.PollyHelper.CreatePolicy();
            for (var i = 0; i < 1000; i++)
            {
                policy.Execute(() => {
                    try
                    {
                        Uri service = serviceProvider.GetServiceBuilder(builder =>
                        {
                            builder.LoadBalancer = TypeLoadBalancer.RoundRobin;
                            builder.ServiceName = "WebApp1";
                            builder.UriScheme = Uri.UriSchemeHttp;
                        }).BuildAsync("/api/test/test").Result;
                        var result = httpClient.GetStringAsync(service).Result;
                        Console.WriteLine($"服务：{service.ToString()},结果：{result}");
                    }
                    catch(Exception ex)
                    {
                        throw ex;
                    }
                });

                Thread.Sleep(1000);
            }

            Console.ReadKey();
        }
    }
}

using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsulRegistration.Extensions
{
    public static class ConsulRegistrationExtensions
    {
        public static void AddConsul(this IServiceCollection services)
        {
            var config = new ConfigurationBuilder().AddJsonFile("ServiceConfig.json").Build();
            services.Configure<ConsulServiceOptions>(config);
        }

        public static IApplicationBuilder UseConsul(this IApplicationBuilder app)
        {
            var hostApplicationLifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();
            var consulServiceOptions = app.ApplicationServices.GetRequiredService<IOptions<ConsulServiceOptions>>().Value;
            consulServiceOptions.ServiceId = Guid.NewGuid().ToString();
            // consul客户端，用于向consul服务器发送指令
            var consulClient = new ConsulClient(consulClientConfig=> {
                consulClientConfig.Address = new Uri(consulServiceOptions.ConsulAddress);
            });
            var features = app.Properties["server.Features"] as FeatureCollection;
            var features2 = app.ServerFeatures as FeatureCollection;
            bool isEqual = features == features2;
            var hostAddress = features.Get<IServerAddressesFeature>().Addresses.First();
            var hostUri = new Uri(hostAddress);
            // 节点服务注册对象
            var registration = new AgentServiceRegistration()
            {
                ID = consulServiceOptions.ServiceId,
                Name = consulServiceOptions.ServiceName,
                Address = hostUri.Host,
                Port = hostUri.Port,
                Check = new AgentServiceCheck
                {
                    // 超时时间
                    Timeout = TimeSpan.FromSeconds(5),
                    // 多久没响应就注销服务
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5),
                    // 健康检查地址
                    HTTP = $"{hostUri.Scheme}://{hostUri.Host}:{hostUri.Port}{consulServiceOptions.HealthCheck}",
                    // 健康检查的间隔
                    Interval=TimeSpan.FromSeconds(10)
                }
            };
            // 注册服务
            consulClient.Agent.ServiceRegister(registration).Wait();
            // 服务终止时，注销服务
            hostApplicationLifetime.ApplicationStopping.Register(() => {
                consulClient.Agent.ServiceDeregister(consulServiceOptions.ServiceId);
            });

            return app;
        }
    }
}

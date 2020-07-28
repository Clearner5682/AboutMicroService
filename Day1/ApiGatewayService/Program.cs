using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ApiGatewayService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureAppConfiguration(builder=> 
                {
                    //builder.AddJsonFile("OcelotBase.json", true, true);//通过配置文件写死的路由获取各个服务
                    //builder.AddJsonFile("OcelotWithConsul.json", true, true);//通过配置的Consul服务获取各个服务
                    //builder.AddJsonFile("OcelotWithDynamicRouting.json", true, true);//动态路由
                    builder.AddJsonFile("OcelotWithIdentityServer.json", true, true);//Ocelot集成Id4
                });
    }
}

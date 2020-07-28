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
                    //builder.AddJsonFile("OcelotBase.json", true, true);//ͨ�������ļ�д����·�ɻ�ȡ��������
                    //builder.AddJsonFile("OcelotWithConsul.json", true, true);//ͨ�����õ�Consul�����ȡ��������
                    //builder.AddJsonFile("OcelotWithDynamicRouting.json", true, true);//��̬·��
                    builder.AddJsonFile("OcelotWithIdentityServer.json", true, true);//Ocelot����Id4
                });
    }
}

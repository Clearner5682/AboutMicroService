using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using ConsulRegistration;
using ConsulRegistration.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace ServiceA
{
    public class Startup
    {

        public Startup(IConfiguration configuration)
        {
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddControllers();
            services.AddConsul();
            services.AddHealthChecks();

            // ��ȡxml�ļ���
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            // ��ȡxml�ļ�·��
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("ServiceA", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "ServiceA", Version = "latest" });
                // ��ȡxml�ļ���
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                // ��ȡxml�ļ�·��
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                // ��ӿ�������ע�ͣ�true��ʾ��ʾ������ע��
                options.IncludeXmlComments(xmlPath, true);

                // api/test/testת��ΪServiceName/api/test/test����Ϊ��Ҫ������һ��ת������������ʵ�ĵ�ַ������ǰ����Ϸ�������
                options.DocInclusionPredicate((docName, apiDesc) => {
                    if (!apiDesc.RelativePath.Contains(docName))
                    {
                        //var values = apiDesc.RelativePath
                        //                    .Split('/')
                        //                    .Select(v => v.Replace("api", "api/" + docName));
                        //apiDesc.RelativePath = string.Join("/", values);

                        apiDesc.RelativePath = docName + "/" + apiDesc.RelativePath;
                    }

                    return true;
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,IOptions<ConsulServiceOptions> options)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            

            app.UseHealthChecks(options.Value.HealthCheck);
            app.UseConsul();

            app.UseSwagger(options=> {
                options.RouteTemplate = "{documentName}/swagger.json";
            });
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/ServiceA/swagger.json", "Docs ServiceA");
            });

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "/api/{controller}/{action}/{id?}");
            });
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ConsulRegistration;
using ConsulRegistration.Extensions;
using Microsoft.Extensions.Options;
using AspectCore.Configuration;
using AspectCore.Extensions.DependencyInjection;
using WebApp1.Services;

namespace WebApp1
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHealthChecks();
            services.AddConsul();

            services.AddControllers().AddControllersAsServices();

            services.AddScoped<IGreetService, GreetService>();
            services.AddScoped<IProductService, ProductService>();

            //services.ConfigureDynamicProxy(config=> {
            //    config.Interceptors.AddTyped<CustomInterceptorAttribute>();
            //    config.Interceptors.AddTyped<HystrixCommandAttribute>(args:new object[] { "fallBack"});
            //});
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,IOptions<ConsulServiceOptions> options)
        {
            app.UseHealthChecks(options.Value.HealthCheck);
            app.UseConsul();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapControllers();
                endpoints.MapControllerRoute("default", "api/{controller}/{action}/{id?}");
            });
        }
    }
}

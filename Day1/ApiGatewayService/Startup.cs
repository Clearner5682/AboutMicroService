using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Ocelot;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;
using Ocelot.Provider.Polly;
using IdentityServer4;
using Microsoft.Extensions.Options;

namespace ApiGatewayService
{
    public class Startup
    {
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //var config = new ConfigurationBuilder().AddJsonFile("Ocelot2.json").Build();
            //services.AddOcelot(configuration);
            services.AddOcelot(configuration)
                    .AddConsul()
                    .AddPolly();

            // Swagger
            services.AddControllers();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc($"Docs All", new OpenApiInfo
                {
                    Title = "Docs All",
                    Version = "latest"
                });
            });

            // IdentityServer
            var authenticationKey = "TestKey";
            services.AddAuthentication("Bearer")
                    .AddIdentityServerAuthentication(authenticationKey, options =>
                    {
                        options.Authority = "http://192.168.0.232:5000";
                        options.ApiName = "ServiceA";
                        options.RequireHttpsMetadata = false;
                        options.SupportedTokens = IdentityServer4.AccessTokenValidation.SupportedTokens.Both;
                    });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "/api/{controller}/{action}/{id?}");
            });
           

            // get from service discovery later
            var serviceList = new List<string>()
            {
                "ServiceA",
                "ServiceB"
            };

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                serviceList.ForEach(service => {
                    options.SwaggerEndpoint($"/{service}/{service}/swagger.json", service);
                });
            });

            

            app.UseOcelot().Wait(); 
        }
    }
}

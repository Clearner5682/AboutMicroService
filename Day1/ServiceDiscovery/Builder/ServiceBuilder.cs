using ServiceDiscovery.LoadBalancer;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ServiceDiscovery.Builder
{
    public class ServiceBuilder : IServiceBuilder
    {
        public ServiceBuilder(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public IServiceProvider ServiceProvider
        {
            get;
            set;
        }

        public string ServiceName 
        {
            get;
            set;
        }
        public string UriScheme 
        {
            get;
            set;
        }
        public ILoadBalancer LoadBalancer 
        {
            get;
            set;
        }

        public async Task<Uri> BuildAsync(string path)
        {
            var services =await ServiceProvider.GetServicesAsync(ServiceName);
            var service = LoadBalancer.Resolve(services);
            var baseUri=new Uri($"{UriScheme}://{service}");

            return new Uri(baseUri,path);
        }
    }
}

using ServiceDiscovery.Builder;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ServiceDiscovery
{
    public static class ServiceProviderExtensions
    {
        public static IServiceBuilder GetServiceBuilder(this IServiceProvider serviceProvider, Action<IServiceBuilder> config)
        {
            var serviceBuilder = new ServiceBuilder(serviceProvider);
            config.Invoke(serviceBuilder);

            return serviceBuilder;
        }
    }
}

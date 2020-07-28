using Consul;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceDiscovery
{
    public class ConsulServiceProvider : IServiceProvider
    {
        private readonly string _serverAddress;

        public ConsulServiceProvider(string serverAddress)
        {
            this._serverAddress = serverAddress;
        }

        public async Task<IList<string>> GetServicesAsync(string serviceName)
        {
            var consulClient = new ConsulClient(c=> {
                c.Address = new Uri(_serverAddress);
            });
            var task= await consulClient.Health.Service(service: serviceName, tag: "", passingOnly: true);

            return task.Response.Select(o => o.Service.Address+":"+o.Service.Port).ToList();
        }
    }
}

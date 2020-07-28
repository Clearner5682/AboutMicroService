using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceDiscovery.LoadBalancer
{
    public class RandomLoadBalancer : ILoadBalancer
    {
        private readonly Random _random = new Random();

        public string Resolve(IList<string> services)
        {
            if (services == null || services.Count == 0)
            {
                throw new ArgumentNullException("没有任何服务");
            }
            var index = _random.Next(services.Count);

            return services[index];
        }
    }
}

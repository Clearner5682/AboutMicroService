using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceDiscovery.LoadBalancer
{
    public class RoundRobinBalancer : ILoadBalancer
    {
        private readonly object lockObj = new object();
        private int _index = 0;

        public string Resolve(IList<string> services)
        {
            lock (lockObj)
            {
                if (services == null || services.Count == 0)
                {
                    throw new ArgumentNullException("没有任何服务");
                }
                if (_index >= services.Count)
                {
                    _index = 0;
                }

                return services[_index++];
            }
        }
    }
}

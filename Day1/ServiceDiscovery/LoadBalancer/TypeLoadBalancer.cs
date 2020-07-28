using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceDiscovery.LoadBalancer
{
    public class TypeLoadBalancer
    {
        public static ILoadBalancer Random = new RandomLoadBalancer();
        public static ILoadBalancer RoundRobin = new RoundRobinBalancer();
    }
}

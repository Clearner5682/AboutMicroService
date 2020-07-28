using System;
using System.Collections.Generic;
using System.Text;

namespace ConsulRegistration
{
    public class ConsulServiceOptions
    {
        public string ConsulAddress { get; set; }
        public string ServiceId { get; set; }
        public string ServiceName { get; set; }
        public string HealthCheck { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp1.Services
{
    public interface IProductService
    {
        [HystrixCommand(nameof(FallBackMethod), IsEnableCircuitBreaker = true, MaxRetryTimes = 2, TimeOutMilliseconds = 2000)]
        IList<string> GetProductList();
        IList<string> FallBackMethod();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp1.Services
{
    public class ProductService : IProductService
    {
        public IList<string> FallBackMethod()
        {
            return new List<string> { "替代产品1", "替代产品2", "替代产品3" };
        }

       
        public IList<string> GetProductList()
        {
            throw new ArgumentNullException("ArgNull");

            return new List<string> {"产品1","产品2","产品3" };
        }
    }
}

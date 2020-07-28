using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp1.Services
{
    public class GreetService : IGreetService
    {
        [CustomInterceptor]
        public Task<string> Greet(string words)
        {
            return Task.FromResult(words);
        }
    }
}

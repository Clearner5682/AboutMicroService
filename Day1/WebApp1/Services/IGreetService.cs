using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp1.Services
{
    public interface IGreetService
    {
        //[CustomInterceptor]
        Task<string> Greet(string words);
    }
}

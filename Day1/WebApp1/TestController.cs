using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AspectCore;
using WebApp1.Services;

namespace WebApp1
{
    public class TestController:ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly IGreetService greetService;
        private readonly IProductService productService;

        public TestController(IConfiguration configuration,IGreetService greetService,IProductService productService)
        {
            this.configuration = configuration;
            this.greetService = greetService;
            this.productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> Test(string words)
        {
            int delay = configuration.GetValue<int>("delay");
            if (delay > 0)
            {
                Thread.Sleep(delay);
            }

            return Ok(await greetService.Greet(words));
        }

        [HttpGet]
        public virtual IActionResult Test2()
        {
            return Ok(productService.GetProductList());
        }

        [HttpGet]
        [CustomInterceptor]
        public virtual string Test3(string words)
        {
            return words;
        }
    }
}

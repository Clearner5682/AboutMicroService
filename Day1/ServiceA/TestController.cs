using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceA
{
    /// <summary>
    /// 测试接口
    /// </summary>
    [Route("api/Test")]
    [ApiController]
    public class TestController:ControllerBase
    {
        private static int _count = 0;
        private readonly IHttpClientFactory httpClientFactory;

        public TestController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// 测试
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Test")]
        public IActionResult Test()
        {
            _count++;
            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}：Get...{_count}");
            if (_count <= 5)
            {
                //Thread.Sleep(2000);
            }

            return Ok(new { ServiceName = this.GetType().FullName, Address = this.HttpContext.Request.Host });
        }

        /// <summary>
        /// 异常测试
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Error")]
        public IActionResult Error()
        {
            throw new Exception("ServiceA throw a Error");
        }

        [HttpGet]
        [Route("LinkCall")]
        public async Task<IActionResult> LinkCall()
        {
            var httpClient = httpClientFactory.CreateClient();
            try
            {
                var result= await httpClient.GetStringAsync("http://192.168.0.232:5002/api/test/test");

                throw new Exception("ServiceA throw a Error");

                return Ok(result);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}

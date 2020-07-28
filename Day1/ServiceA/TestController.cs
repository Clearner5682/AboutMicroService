using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspectCore;
using AspectCore.DynamicProxy;

namespace WebApp1
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CustomInterceptorAttribute : AbstractInterceptorAttribute
    {
        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            try
            {
                Console.WriteLine("Before Service Call");
                var serviceMethod = context.ServiceMethod;
                var parameters = serviceMethod.GetParameters();
                var returnType = serviceMethod.ReturnType;
                await next(context);
                if (serviceMethod.IsReturnTask())
                {
                    dynamic oldValue = context.ReturnValue;
                    Console.WriteLine($"旧的返回值：{oldValue.Result}");
                    context.ReturnValue =Task.FromResult("你好啊，李银河");// 替换原来的返回值
                    dynamic newValue = context.ReturnValue;
                    Console.WriteLine($"新的返回值：{newValue.Result}");
                }
                else
                {
                    Console.WriteLine($"旧的返回值：{context.ReturnValue}");
                    context.ReturnValue = "你好啊，李银河";// 替换原来的返回值
                    Console.WriteLine($"新的返回值：{context.ReturnValue}");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Service throw an Exception");
                throw ex;
            }
            finally
            {
                Console.WriteLine("After Service Call");
            }
        }
    }
}

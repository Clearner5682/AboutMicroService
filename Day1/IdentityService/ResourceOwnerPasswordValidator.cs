using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityService
{
    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            await Task.Run(() =>
            {
                if (context.UserName == "admin" && context.Password == "1")
                {
                    var claims = new List<Claim> {
                            new Claim("UserName","admin"),
                            new Claim("Address","guangdong"),
                            new Claim("Age","29")
                        };
                    context.Result = new GrantValidationResult(Guid.NewGuid().ToString(), "password", claims);
                }
                else
                {
                    context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest, "用户名或密码不正确");
                }
            });
        }
    }
}

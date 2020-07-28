using IdentityServer4.Models;
using IdentityServer4.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityService
{
    public class ProfileService : IProfileService
    {
        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            await Task.Run(() => {
                try
                {
                    var claims = context.Subject.Claims.ToList();
                    context.IssuedClaims = claims;
                }
                catch (Exception ex)
                {
                    //记录日志
                }
            });
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            await Task.Run(() => {
                context.IsActive = true;
            });
        }
    }
}

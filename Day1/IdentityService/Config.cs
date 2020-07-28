using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4;
using IdentityServer4.Models;

namespace IdentityService
{
    public class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResource()
        {
            return new List<IdentityResource>
             {
                 new IdentityResources.OpenId(),
                 new IdentityResources.Profile(),
                 new IdentityResources.Email()
             };
        }
        public static IEnumerable<ApiResource> GetApiResource()
        {
            return new List<ApiResource>
            {
                new ApiResource("ServiceA","ServiceA"),
                new ApiResource("ServiceB","ServiceB")
            };
        }
        public static IEnumerable<Client> GetClients()
        {
            var clientList = new List<Client> {
                new Client
              {
                    AlwaysSendClientClaims=true,
                    ClientId="self",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    ClientSecrets = { new Secret("self".Sha256()) },
                     RefreshTokenUsage=TokenUsage.ReUse,
                     RefreshTokenExpiration=TokenExpiration.Absolute,
                     AbsoluteRefreshTokenLifetime=60 * 60 * 24 * 7,
                     AlwaysIncludeUserClaimsInIdToken=true,
                     AllowOfflineAccess = true,
                     AccessTokenLifetime=(int)TimeSpan.FromMinutes(60).TotalSeconds,
                    AllowedScopes=new List<string>
                    {
                        "ServiceA",
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.OfflineAccess
                    }

              }
            };

            return clientList;
        }
    }
}

// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityModel;
using IdentityServer4.Test;
using System.Collections.Generic;
using System.Security.Claims;

namespace IdentityServer4.Quickstart.UI
{
    public class TestUsers
    {
        public static List<TestUser> Users = new List<TestUser>
        {
            new TestUser{SubjectId = "d860efca-22d9-47fd-8249-791ba61b07c7", Username = "Frank", Password = "P@ssw0rd!", 
                Claims = 
                {
                    new Claim(JwtClaimTypes.Name, "Frank Underwood"),
                    new Claim(JwtClaimTypes.GivenName, "Frank"),
                    new Claim(JwtClaimTypes.FamilyName, "Underwood"),
                    new Claim(JwtClaimTypes.Email, "frank.underwood@email.com"),
                    new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                    new Claim(JwtClaimTypes.WebSite, "http://frank.com"),
                    new Claim(JwtClaimTypes.Address, "One Main Road"),
                    new Claim("role", "FreeUser")
                }
            },
            new TestUser{SubjectId = "b7539694-97e7-4dfe-84da-b4256e1ff5c7", Username = "Claire", Password = "P@ssw0rd!", 
                Claims = 
                {
                    new Claim(JwtClaimTypes.Name, "Claire Underwood"),
                    new Claim(JwtClaimTypes.GivenName, "Claire"),
                    new Claim(JwtClaimTypes.FamilyName, "Underwood"),
                    new Claim(JwtClaimTypes.Email, "claire.underwood@email.com"),
                    new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                    new Claim(JwtClaimTypes.WebSite, "http://claire.com"),
                    new Claim(JwtClaimTypes.Address, "One Big Street"),
                    new Claim("role", "PayingUser")
                }
            }
        };
    }
}
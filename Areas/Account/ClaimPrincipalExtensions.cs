using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace eInvoiceApp
{
    public static class ClaimPrincipalExtensions
    {
                
       
        public static string GetClaim(this ClaimsPrincipal user, string ClaimType)
        {            
            if (user !=null)
            {
                foreach (Claim item in user.Claims)
                {
                    if (item.Type == ClaimType)
                    {
                        return item.Value;
                    }
                }
            }

            return string.Empty;
        }

    }
}

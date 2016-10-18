using Microsoft.Azure.Mobile.Server.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Web.Http;

namespace ContosoInsurance.API.Controllers
{
    [MobileAppController]
    public class TestController : ApiController
    {
        // GET: api/Test
        public string Get()
        {
            return GetUserId(User);
        }

        private string GetUserId(IPrincipal user)
        {
            ClaimsPrincipal claimsUser = (ClaimsPrincipal)user;

            string provider = claimsUser.FindFirst("http://schemas.microsoft.com/identity/claims/identityprovider").Value;
            string sid = claimsUser.FindFirst(ClaimTypes.NameIdentifier).Value;

            // The above assumes WEBSITE_AUTH_HIDE_DEPRECATED_SID is true. Otherwise, use the stable_sid claim:
            // string sid = claimsUser.FindFirst("stable_sid").Value; 

            return $"{provider}|{sid}";
        }
    }
}

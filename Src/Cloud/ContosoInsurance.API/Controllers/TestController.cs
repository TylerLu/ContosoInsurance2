using Microsoft.Azure.Mobile.Server.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;

namespace ContosoInsurance.API.Controllers
{
    [MobileAppController]
    public class TestController : ApiController
    {
        // GET: api/Test
        public string Get()
        {
            ClaimsPrincipal claimsUser = (ClaimsPrincipal)this.User;
            string id = claimsUser.FindFirst(ClaimTypes.NameIdentifier).Value;
            return id;
        }
    }
}

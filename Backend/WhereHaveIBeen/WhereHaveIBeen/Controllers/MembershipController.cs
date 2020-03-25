using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WhereHaveIBeen.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MembershipController : ControllerBase
    {
        [HttpGet]
        public string Get()
        {
            return "Hi";
        }
    }
}
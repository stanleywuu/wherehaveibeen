using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Data;
using Application.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Storage;

namespace WhereHaveIBeen.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VisitController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<string>> Get()
        {
            var conn = ContextProvider.Conn;
            var users = await conn.QueryAsync<Visit>("");
            StringBuilder sb = new StringBuilder();

            foreach (var user in users)
            {
                sb.AppendLine(JsonConvert.SerializeObject(user));
            }

            var result = new OkObjectResult(sb.ToString());
            return result;
        }

        [HttpPost]
        public async Task<ActionResult<string>> Create([FromBody]VisitRequest request)
        {
            var conn = ContextProvider.Conn;
            await conn.InsertAsync(request.ToPersistedData());

            return new OkObjectResult("Visit has been logged");
        }
    }
}
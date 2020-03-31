using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Application.Data;
using Application.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Storage;

namespace WhereHaveIBeen.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class VisitController : ControllerBase
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        public VisitController(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public async Task<ActionResult<string>> Get([FromQuery] int userId = 0)
        {
            // TODO: Make sure userId IS current user
            var conn = ContextProvider.Conn;
            var users = await conn.Table<Visit>().Where(v => v.UserId == userId).ToListAsync();

            StringBuilder sb = new StringBuilder();

            var text = JsonConvert.SerializeObject(users);

            var result = new OkObjectResult(text);
            return result;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult<string>> Create([FromBody]VisitRequest request)
        {
            var conn = ContextProvider.Conn;
            await conn.InsertAsync(await request.ToPersistedData());

            return new OkObjectResult("Visit has been logged");
        }

        [HttpPost, Route("delete/{userId}"), Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> DeleteData(int userId)
        {
            var claims = httpContextAccessor.HttpContext.User;
            var claim = claims.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null && int.TryParse(claim.Value, out int claimUserId))
            {
                if (userId != claimUserId)
                {
                    return Unauthorized();
                }

                var conn = ContextProvider.Conn;
                var visitsToDelete = await conn.Table<Visit>().Where(v => v.UserId == userId).ToArrayAsync();

                await conn.RunInTransactionAsync((c) =>
                {
                    foreach (var visit in visitsToDelete)
                    {
                        c.Delete(visit);
                    }
                });
            }
            else return Unauthorized();

            return new OkResult();
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Application;
using Application.Data;
using Application.Requests;
using Application.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Storage;

namespace WhereHaveIBeen.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MembershipController : ControllerBase
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        public MembershipController(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        // Ultra secret debug link, lol
        [HttpGet]
        public async Task<ActionResult<string>> Get()
        {
            var conn = ContextProvider.Conn;
            var users = await conn.QueryAsync<User>("");
            StringBuilder sb = new StringBuilder();

            foreach (var user in users)
            {
                sb.AppendLine(JsonConvert.SerializeObject(user));
            }

            var result = new OkObjectResult(sb.ToString());
            return result;
        }

        /// <summary>
        /// POST /membership HTTP/1.1
        /// Host: https://wherehaveibeen.azurewebsites.net
        /// Content-Type: application/json
        /// {"Username":"admin","Email":"swu@swu.com","Password":"blah","Token":null}
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<string>> Create([FromBody]UserRequest request)
        {
            var conn = ContextProvider.Conn;
            await conn.InsertAsync(request.ToPersistedData());

            return new OkObjectResult("User has been created");
        }

        /// <summary>
        /// curl --location --request POST 'https://wherehaveibeen.azurewebsites.net/membership/login' \
        /// 'Content-Type: application/json' \
        /// '{"Username":"admin","Password":"blah"}'
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("login")]
        public async Task<ActionResult<TokenResponse>> Login([FromBody] AuthRequest request)
        {
            /* POST /membership/login HTTP/1.1
            Host: https://wherehaveibeen.azurewebsites.net
            Content-Type: application/json

            {"Username":"admin","Password":"blah"}
            */

            var conn = ContextProvider.Conn;
            var hashedPassword = request.Password.Encrypt();
            var existingUsers =
                await conn.Table<User>()
                .Where(u => u.Username == request.Username &&
                    u.Password == hashedPassword).ToArrayAsync();

            var user = existingUsers.First();
            // it's good if it crashes here, we don't have to handle it ourselves

            var token = AuthenticationUtilities.GenerateToken(user.UserId, user.Username);
            user.Token = token;

            await conn.InsertOrReplaceAsync(user);

            var response = new TokenResponse()
            {
                UserId = user.UserId,
                Token = token
            };

            return new ActionResult<TokenResponse>(response);
        }

        [HttpPost("corona")]
        public async Task<ActionResult> MarkAsPositive([FromBody] UserAtRiskRequest request)
        {
            var userId = request.UserId;
            var conn = ContextProvider.Conn;
            User user = null;
            try
            {
                user = await conn.GetAsync<User>(userId);
            }
            catch
            {
                return Unauthorized();
            }

            request.ToPersistedData(user);
            await conn.RunInTransactionAsync((c) =>
            {
                var targetedDay = DateTime.Today.AddDays(-18);
                var affectedVisits = c.Table<Visit>().Where(v =>
                    v.UserId == userId &&
                    v.CheckIn > targetedDay)
                .ToList();

                // we could either do this, or we could join, this is easier for now, let's set it
                // this is all throw away since we can't connect to a sql instance
                foreach (var visit in affectedVisits)
                {
                    visit.AtRisk = true;
                }

                c.UpdateAll(affectedVisits);
                c.Update(user);
            });
            return new OkResult();
        }

        [HttpPost("token")]
        public async Task<ActionResult<int>> LoginWithToken([FromBody] string token)
        {
            var now = DateTime.Now;

            var conn = ContextProvider.Conn;
            var query = conn.Table<User>().Where(u => u.Token == token && (u.TokenExpiry == null || u.TokenExpiry < now));
            var user = await query.FirstOrDefaultAsync();

            if (user == null)
            {
                return Unauthorized();
            }

            return new ObjectResult(user.UserId);
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
                User user = null;
                try
                {
                    user = await conn.GetAsync<User>(userId);
                }
                catch
                { }
                await conn.RunInTransactionAsync((c) =>
                {
                    foreach (var visit in visitsToDelete)
                    {
                        c.Delete(visit);
                    }

                    if (user != null)
                    {
                        c.Delete(user);
                    }
                });
            }
            else return Unauthorized();

            return new OkResult();
        }
    }
}
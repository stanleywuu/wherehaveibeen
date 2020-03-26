using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application;
using Application.Data;
using Application.Requests;
using Application.Response;
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

            var token = Guid.NewGuid().ToString().Encrypt();
            user.Token = token;

            await conn.InsertOrReplaceAsync(user);

            var response = new TokenResponse()
            {
                Token = token
            };

            return new ActionResult<TokenResponse>(response);
        }

        [HttpPost("token")]
        public async Task<ActionResult> LoginWithToken(string token)
        {
            var now = DateTime.Now;

            var conn = ContextProvider.Conn;
            var query = conn.Table<User>().Where(u => u.Token == token && (u.TokenExpiry == null || u.TokenExpiry < now));
            var user = await query.FirstOrDefaultAsync();

            if (user == null)
            {
                return Unauthorized();
            }
            return new OkResult();
        }
    }
}
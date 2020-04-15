using Application;
using Application.Data;
using Application.Requests;
using Application.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Storage;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

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

        /// <summary>
        /// POST /membership HTTP/1.1
        /// Host: https://wherehaveibeen.azurewebsites.net
        /// Content-Type: application/json
        /// {"Username":"admin","Email":"swu@swu.com","Password":"blah","Token":null}
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<string>> Create([FromBody]UserRequest request)
        {
            await request.ToPersistedData().Insert();

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
            var hashedPassword = request.Password.Encrypt();
            var existingUsers = await UserAccess.GetByUsernamePassword(request.Username, hashedPassword);
            var user = existingUsers.First();

            var expiry = DateTime.Now.AddDays(1);
            var token = AuthenticationUtilities.GenerateToken(user.UserId, user.Username, expiry);
            user.Token = token;

            await user.UpdateAsync();

            var response = new TokenResponse()
            {
                UserId = user.UserId,
                Token = token,
                Expires = expiry.ToUniversalTime(),
                Reported = user.AtRisk
            };

            return new ActionResult<TokenResponse>(response);
        }

        [HttpPost("notcorona")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> UndoCorona([FromQuery] int userId)
        {
            User user = null;

            // You can only mark yourself as having corona
            if (UserUtilities.GetUserId(httpContextAccessor.HttpContext.User) != userId)
            {
                return Unauthorized();
            }

            user = await DataAccess.Get<User, int>(userId);
            user.AtRisk = false;
            await user.UpdateAsync();

            try
            {
                await VisitAccess.UpdateVisitStatus(userId, SqlConstants.DateTimeMin, false);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to update user as not having covid", ex);
            }

            return new OkResult();
        }

        [HttpPost("recovered")]
        public async Task<ActionResult> MarkAsRecovered([FromBody] UserAtRiskRequest request)
        {
            var userId = request.UserId;
            User user = null;
            try
            {
                user = await DataAccess.Get<User, int>(userId);
                if (!user.AtRisk)
                {
                    return new OkResult();
                }
            }
            catch
            {
                return Unauthorized();
            }

            user.AtRisk = false;
            await user.UpdateAsync();
            return new OkResult();
        }

        [HttpPost("corona")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> MarkAsPositive([FromBody] UserAtRiskRequest request)
        {
            var userId = request.UserId;
            User user = null;

            // You can only mark yourself as having corona
            if (UserUtilities.GetUserId(httpContextAccessor.HttpContext.User) != userId)
            {
                return Unauthorized();
            }

            try
            {
                user = await DataAccess.Get<User, int>(userId);
                if (user.AtRisk)
                {
                    return new OkResult();
                }
            }
            catch
            {
                return Unauthorized();
            }

            request.ToPersistedData(user);

            var targetedDay = DateTime.Today.AddDays(-18);
            await VisitAccess.UpdateVisitStatus(userId, targetedDay);

            user.AtRisk = true;
            await user.UpdateAsync();

            try
            {
                var affectedUserIds = await RiskAccess.GetUsersAffectedBy(userId, targetedDay);
                await RiskAccess.PrepareUsersForNotification(affectedUserIds, DateTime.Now);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to notify user", ex);
            }

            return new OkResult();
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

                var user = await DataAccess.Get<User, int>(userId);

                if (user != null)
                {
                    await VisitAccess.DeleteVisitsByUser(user.UserId);
                }
            }
            else return Unauthorized();

            return new OkResult();
        }
    }
}
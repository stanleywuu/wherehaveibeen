using System;
using System.Collections.Generic;
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
            var response = await VisitAccess.GetVisitResponseFor(userId);

            StringBuilder sb = new StringBuilder();

            var text = JsonConvert.SerializeObject(response);

            var result = new OkObjectResult(text);
            return result;
        }

        [HttpGet("risk")]
        public async Task<ActionResult<IList<RiskyVisitResponse>>> GetForVisit([FromQuery] int visitId)
        {
            var conn = ContextProvider.Conn;
            var visit = await conn.GetAsync<Visit>(visitId);
            var visits = await RiskAccess.GetRiskyVisitsFor(visit);
            var response = new List<RiskyVisitResponse>();

            foreach (var item in visits)
            {
                var riskyVisit = new RiskyVisitResponse()
                {
                    VisitId = item.VisitId,
                    Address = item.Address,
                    CheckIn = item.CheckIn,
                    CheckOut = item.CheckOut.GetValueOrDefault(item.CheckIn),
                    Latitude = item.Latitude,
                    Longitude = item.Longitude,
                    DistanceInKm =
                    Convert.ToInt32(GPSExtensions.GetDistance(visit.Longitude, visit.Latitude, item.Longitude, item.Latitude) / 1000)
                };
                response.Add(riskyVisit);
            }

            return new OkObjectResult(response);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult<string>> Create([FromBody]VisitRequest request)
        {
            var conn = ContextProvider.Conn;
            var user = await conn.GetAsync<User>(request.UserId);
            var entity = await request.ToPersistedData();
            entity.AtRisk = user.AtRisk;

            await conn.InsertAsync(entity);

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
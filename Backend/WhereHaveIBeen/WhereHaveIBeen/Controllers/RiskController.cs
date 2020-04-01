using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application;
using Application.Data;
using Application.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Storage;

namespace WhereHaveIBeen.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RiskController : ControllerBase
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public RiskController(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public async Task<ActionResult<ICollection<Visit>>> GetRisks([FromQuery]double lat, [FromQuery]double lng, [FromQuery]DateTime? from, [FromQuery]DateTime? to)
        {
            var riskyVisits = await UserAccess.GetAtRiskVisits(lat, lng, from, to);
            return new OkObjectResult(riskyVisits);
        }

        [HttpGet("user")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult<RiskyVisitResponse>> GetRisks([FromQuery]int userId, [FromQuery]double lat, [FromQuery]double lng, [FromQuery]DateTime? from, [FromQuery]DateTime? to)
        {
            var claimUserId = UserUtilities.GetUserId(httpContextAccessor.HttpContext.User);
            if (claimUserId != userId)
            {
                //return Unauthorized();
            }
            var riskyVisits = await UserAccess.GetAtRiskVisits(userId, from, to);
            var riskyResponse = new List<RiskyVisitResponse>();
            var groupedRiskyVisits = new Dictionary<int, RiskyVisitResponse>();

            if (riskyVisits.Count > 0)
            {
                foreach (var visit in riskyVisits.OrderBy(v => v.VisitId).ToList())
                {
                    if (!groupedRiskyVisits.ContainsKey(visit.VisitId))
                    {
                        var riskyVisit = new RiskyVisitResponse()
                        {
                            VisitId = visit.VisitId,
                            Address = visit.Address,
                            CheckIn = visit.CheckIn,
                            CheckOut = visit.CheckOut,
                            Latitude = visit.Latitude,
                            Longitude = visit.Longitude,
                            DistanceInKm = 0,
                            LinkedVisits = new List<RiskyVisitResponse>()
                        };

                        groupedRiskyVisits.Add(visit.VisitId, riskyVisit);
                    }

                    var relatedVisit = new RiskyVisitResponse()
                    {
                        Address = visit.Address2,
                        Latitude = visit.Latitude2,
                        Longitude = visit.Longitude2,
                        CheckIn = visit.CheckIn2,
                        CheckOut = visit.CheckOut2,
                        DistanceInKm = Convert.ToInt32(GPSExtensions.GetDistance(visit.Longitude, visit.Latitude, visit.Longitude2, visit.Latitude2) / 1000)
                    };

                    groupedRiskyVisits[visit.VisitId].LinkedVisits.Add(relatedVisit);
                }

            }

            var response = groupedRiskyVisits.Select(g => g.Value).ToList();

            return new OkObjectResult(response);
        }

    }
}
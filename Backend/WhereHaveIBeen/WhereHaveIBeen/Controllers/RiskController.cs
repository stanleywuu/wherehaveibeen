using Application;
using Application.Data;
using Application.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            var riskyResponses = riskyVisits.Select(v =>
            new
            {
                Address = v.Address,
                PlaceName = string.IsNullOrEmpty(v.PlaceName) ? v.Address : v.PlaceName,
                Latitude = v.Latitude,
                Longitude = v.Longitude,
                CheckIn = v.CheckIn,
                CheckOut = v.CheckOut
            }).OrderBy(r => r.PlaceName);

            return new OkObjectResult(riskyResponses);
        }

        [HttpGet("visit")]
        public async Task<ActionResult<string>> GetRiskyVisits([FromQuery]int userId, [FromQuery]double lat, [FromQuery]double lng)
        {
            var startDate = DateTime.Now.AddDays(-15);
            var riskyVisits = await UserAccess.GetAtRiskVisitsByVisit(userId, lat, lng, startDate);
            var riskyResponse = riskyVisits.Select(v =>
            {
                return new
                {
                    CheckIn = v.CheckIn,
                    CheckOut = v.CheckOut,
                    Distance = Math.Round(GPSExtensions.GetDistanceInMeters(lng, lat, v.Longitude, v.Latitude), 2)
                };
            });

            return new OkObjectResult(JsonConvert.SerializeObject(riskyResponse.OrderBy(v => v.Distance)));
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

                    var distance = GPSExtensions.GetDistanceInMeters(visit.Longitude, visit.Latitude, visit.Longitude2, visit.Latitude2);
                    var relatedVisit = new RiskyVisitResponse()
                    {
                        Address = visit.Address2,
                        Latitude = visit.Latitude2,
                        Longitude = visit.Longitude2,
                        CheckIn = visit.CheckIn2,
                        CheckOut = visit.CheckOut2,
                        DistanceInKm = Convert.ToInt32(distance)
                    };

                    groupedRiskyVisits[visit.VisitId].LinkedVisits.Add(relatedVisit);
                }

            }

            var response = groupedRiskyVisits.Select(g => g.Value).ToList();

            return new OkObjectResult(response);
        }

    }
}
using Application.Data;
using Application.Response;
using Storage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Application.Requests
{
    public class VisitAccess
    {
        public static async Task<IList<VisitResponse>> GetVisitResponseFor(int userId)
        {
            var conn = ContextProvider.Conn;
            var visits = await conn.Table<Visit>().Where(v => v.UserId == userId).ToListAsync();
            var response = new List<VisitResponse>();

            foreach (var visit in visits)
            {
                var riskyInteractions = await RiskAccess.GetRiskyVisitsFor(visit);
                response.Add(new VisitResponse()
                {
                    Address = visit.Address,
                    CheckIn = visit.CheckIn,
                    CheckOut = visit.CheckOut,
                    Latitude = visit.Latitude,
                    Longitude = visit.Longitude,
                    PlaceName = visit.PlaceName,
                    VisitId = visit.VisitId,
                    AtRisk = visit.AtRisk,
                    RiskyInteractions = riskyInteractions == null ? 0 : riskyInteractions.Count
                });
            }

            return response;
        }
    }
}

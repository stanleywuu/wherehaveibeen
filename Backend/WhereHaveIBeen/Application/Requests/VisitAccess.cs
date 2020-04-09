using Application.Data;
using Application.Response;
using Dapper;
using Dapper.Contrib.Extensions;
using Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Requests
{
    public class VisitAccess
    {
        public static async Task<IList<VisitResponse>> GetVisitResponseFor(int userId)
        {
            using (var conn = ContextProvider.Instance.Conn)
            {
                var visits = await conn.QueryAsync<Visit>(
                    @"SELECT * FROM Visits v WHERE v.UserId = @UserId",
                    new { UserId = userId });

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
                        RiskyInteractions = riskyInteractions == null ? 0 : riskyInteractions.Count()
                    });
                }

                return response;
            }
        }

        public static async Task DeleteVisitsByUser(int userId)
        {
            using (var conn = ContextProvider.Instance.Conn)
            {
                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        var visits = await conn.QueryAsync<Visit>(@"
SELECT * FROM VISITS v WHERE v.UserId = @UserId", new { UserId = userId });

                        foreach (var visit in visits)
                        {
                            await conn.DeleteAsync(visit);
                        }
                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                    }
                }
            }
        }

        public static async Task MarkVisitsAsRisky(int userId, DateTime targetedDay)
        {
            using (var conn = ContextProvider.Instance.Conn)
            {
                conn.Open();

                var affectedVisits = await conn.QueryAsync<Visit>(@"
SELECT * FROM Visits v WHERE
    v.UserId = @UserId AND
    v.CheckIn > @TargetedDay",
        new { UserId = userId, TargetedDay = targetedDay });


                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        // we could either do this, or we could join, this is easier for now, let's set it
                        // this is all throw away since we can't connect to a sql instance
                        foreach (var visit in affectedVisits)
                        {
                            visit.AtRisk = true;
                            conn.Update(visit, trans);
                        }
                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                    }
                }

                conn.Close();
            }
        }
    }
}

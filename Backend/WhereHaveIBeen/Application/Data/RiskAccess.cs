using Dapper;
using Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Data
{
    public class RiskAccess
    {
        public static async Task PrepareUsersForNotification(ICollection<int> userIds, DateTime issuedDate)
        {
            var conn = ContextProvider.Instance.Conn;
            // Find existing notifications for the users to be sent to
            await Task.Delay(0);
            conn.Close();
        }

        public static async Task<ICollection<int>> GetUsersAffectedBy(int userId, DateTime startDate)
        {
            using (var conn = ContextProvider.Instance.Conn)
            {

                var visits = await conn.QueryAsync<Visit>(@"select v1.*
from visits v1
inner join visits v2
    ON (v2.latitudeRounded >= (v1.latitudeRounded - 0.001) AND v2.latitudeRounded <= (v1.latitudeRounded + 0.001))
    AND (v2.longitudeRounded >= (v1.longitudeRounded - 0.001) AND v2.longitudeRounded <= (v1.longitudeRounded + 0.001))
    AND v1.userid <> v2.userid
    AND v2.AtRisk = 1
WHERE v2.userId = @userId
AND v2.CheckIn > @checkIn
AND ((v2.CheckIn >= v1.CheckIn AND V2.CheckIn <= v1.CheckOut) OR
     (v2.CheckIn <= v1.CheckIn AND v2.CheckOut >= v1.CheckIn))",
    new { userId = userId, checkIn = startDate });

                return (visits != null && visits.Any()) ? visits.Select(v => v.UserId).Distinct().ToList() :
                    new List<int>();
            }
        }

        public static async Task<IList<Visit>> GetRiskyVisitsFor(Visit visit)
        {
            using (var conn = ContextProvider.Instance.Conn)
            {
                var visits = await conn.QueryAsync<Visit>
                    ($@"
select * from Visits
where AtRisk = 1 and UserId != @UserId
and latituderounded >= ({visit.LatitudeRounded - 0.001}) AND latituderounded <= ({visit.LatitudeRounded + 0.001}) 
and longituderounded >= ({visit.LongitudeRounded - 0.001}) AND longituderounded <= ({visit.LongitudeRounded + 0.001})
and
((checkin <= @CheckIn AND checkout >= @CheckIn) OR
(checkin >= @CheckIn AND checkIn <= @CheckOut))
",
    // Visit = target visit
    // so if a risky visit happened before user checked in and left after the user checked out
    // or (if a risky visit happened after user checked in, but before user checked out)
    // then it's risky
    new { UserId = visit.UserId, CheckIn = visit.CheckIn, CheckOut = visit.CheckOut.Value }
                    );

                return visits.ToList();
            }
        }

        public static async Task<bool> IsPersonAtRisk(int userId)
        {
            using (var conn = ContextProvider.Instance.Conn)
            {
                var query = await conn.QueryAsync<PersonAtRisk>("" +
                    @"
SELECT * FROM peopleatrisk p WHERE
p.UserId = @UserId AND p.SeenNotification IS null", new { UserId = userId });

                return query.Count() > 0;
            }
        }

    }
}

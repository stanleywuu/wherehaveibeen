using Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Data
{
    public class RiskAccess
    {
        public static async Task PrepareUsersForNotification(ICollection<int> userIds, DateTime issuedDate)
        {
            var conn = ContextProvider.Conn;
            // Find existing notifications for the users to be sent to
            await conn.RunInTransactionAsync((c) =>
            {
                var existingIds = c.Table<PersonAtRisk>().Where(p => userIds.Contains(p.UserId) &&
                    p.WarningIssuedDate < issuedDate).ToList().Select(p => p.UserId).ToList();

                var idsToSendTo = userIds.Except(existingIds).ToList();
                foreach (var id in idsToSendTo)
                {
                    var entity = new PersonAtRisk()
                    {
                        UserId = id,
                        WarningIssuedDate = issuedDate
                    };

                    c.Insert(entity);
                }
            });
        }

        public static async Task<ICollection<int>> GetUsersAffectedBy(int userId, DateTime startDate)
        {
            var conn = ContextProvider.Conn;
            var visits = await conn.QueryAsync<Visit>(@"select v1.*
from visit v1
inner join visit v2
    ON (v2.latitudeRounded >= (v1.latitudeRounded - 0.001) AND v2.latitudeRounded <= (v1.latitudeRounded + 0.001))
    AND (v2.longitudeRounded >= (v1.longitudeRounded - 0.001) AND v2.longitudeRounded <= (v1.longitudeRounded + 0.001))
    AND v1.userid <> v2.userid
    AND v2.AtRisk = 1
WHERE v2.userId = ?
AND v2.CheckIn > ?
AND ((v2.CheckIn >= v1.CheckIn AND V2.CheckIn <= v1.CheckOut) OR
     (v2.CheckIn <= v1.CheckIn AND v2.CheckOut >= v1.CheckIn))",
userId, startDate.Ticks);

            return (visits != null && visits.Any()) ? visits.Select(v => v.UserId).Distinct().ToList() :
                new List<int>();
        }

        public static async Task<IList<Visit>> GetRiskyVisitsFor(Visit visit)
        {
            var conn = ContextProvider.Conn;
            var visits = await conn.QueryAsync<Visit>
                ($@"
select * from Visit
where AtRisk = 1 and UserId != ?
and latituderounded >= ({visit.LatitudeRounded - 0.001}) AND latituderounded <= ({visit.LatitudeRounded + 0.001}) 
and longituderounded >= ({visit.LongitudeRounded - 0.001}) AND longituderounded <= ({visit.LongitudeRounded + 0.001})
and
((checkin <= ? AND checkout >= ?) OR
(checkin >= ? AND checkIn <= ?))
",
// Visit = target visit
// so if a risky visit happened before user checked in and left after the user checked out
// or (if a risky visit happened after user checked in, but before user checked out)
// then it's risky
visit.UserId,
visit.CheckIn.Ticks, visit.CheckIn.Ticks,
visit.CheckIn.Ticks, visit.CheckOut.Value.Ticks
                );
            return visits;
        }

        public static async Task<bool> IsPersonAtRisk(int userId)
        {
            var conn = ContextProvider.Conn;
            var query = conn.Table<PersonAtRisk>().Where(p => p.UserId == userId & p.SeenNotification == null);
            return await query.CountAsync() > 0;
        }

    }
}

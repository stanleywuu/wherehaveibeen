using Dapper;
using Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Data
{
    public class UserAccess
    {
        public static async Task<IEnumerable<User>> GetByUsernamePassword(string username, string password)
        {
            using (var conn = ContextProvider.Instance.Conn)
            {
                var query = await conn.QueryAsync<User>(
     @"SELECT * FROM USERS u
WHERE u.Username = @Username AND
u.Password = @Password",
     new { Username = username, Password = password });

                return query;
            }
        }

        public static async Task<ICollection<Visit>> GetAtRiskVisits(double lat, double lng, DateTime? from, DateTime? to)
        {
            using (var conn = ContextProvider.Instance.Conn)
            {
                var lat2D = (double)Math.Round((decimal)lat, 2);
                var lng2D = (double)Math.Round((decimal)lng, 2);

                var startDate = from ?? DateTime.Now.AddDays(-15);
                var query = await conn.QueryAsync<Visit>(@"
SELECT * FROM VISITS v WHERE
    v.AtRisk = 1 AND
    v.Latitude2Decimal = @Lat2D AND
    v.Longitude2Decimal = @Lng2D AND
    v.CheckIn > @StartDate",
                new { Lat2D = lat2D, Lng2D = lng2D, StartDate = startDate });

                return query.ToList();
            }
        }

        public static async Task<ICollection<RiskyVisit>> GetAtRiskVisits(int userId, DateTime? from, DateTime? to)
        {
            using (var conn = ContextProvider.Instance.Conn)
            {

                var startDate = from ?? DateTime.Now.AddDays(-15);
                var result = await conn.QueryAsync<RiskyVisit>(@"
select v1.VisitId, v1.CheckIn, v1.CheckOut as CheckOut, v1.Address as Address, v1.Latitude as Latitude, v1.Longitude as Longitude,
v2.CheckIn as CheckIn2, v2.CheckOut as CheckOut2, v2.Address as Address2, v2.Latitude as Latitude2, v2.Longitude as Longitude2
from visits v1 
inner join visits v2 
    ON (v2.latitudeRounded >= (v1.latitudeRounded - 0.001) AND v2.latitudeRounded <= (v1.latitudeRounded + 0.001))
    AND (v2.longitudeRounded >= (v1.longitudeRounded - 0.001) AND v2.longitudeRounded <= (v1.longitudeRounded + 0.001))
    AND v2.userid <> v1.userid
    AND v2.AtRisk = 1
where v1.userid = @UserId
AND v1.CheckIn > @StartDate
AND ((v2.CheckIn >= v1.CheckIn AND V2.CheckIn <= v1.CheckOut) OR
     (v2.CheckIn <= v1.CheckIn AND v2.CheckOut >= v1.CheckIn))
", new { UserId = userId, StartDate = startDate });
                return result.ToList();
            }
        }
    }
}

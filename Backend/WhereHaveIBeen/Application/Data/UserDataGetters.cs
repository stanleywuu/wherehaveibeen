using Storage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Application.Data
{
    public class UserDataGetters
    {
        public static async Task<ICollection<Visit>> GetAtRiskVisits(double lat, double lng, DateTime? from, DateTime? to)
        {
            var conn = ContextProvider.Conn;
            var lat2D = (double)Math.Round((decimal)lat, 2);
            var lng2D = (double)Math.Round((decimal)lng, 2);

            var startDate = from ?? DateTime.Now.AddDays(-15);
            var query = conn.Table<Visit>().Where(
                v => v.AtRisk == true && v.Latitude2Decimal == lat2D &&
                v.Longitude2Decimal == v.Longitude2Decimal &&
                v.CheckIn > startDate);

            return await query.ToListAsync();
        }

        public static async Task<ICollection<RiskyVisit>> GetAtRiskVisits(int userId, DateTime? from, DateTime? to)
        {
            var conn = ContextProvider.Conn;

            var startDate = from ?? DateTime.Now.AddDays(-15);
            var result = await conn.QueryAsync<RiskyVisit>(@"
select v1.VisitId, v1.CheckIn, v1.CheckOut as CheckOut, v1.Address as Address, v1.Latitude as Latitude, v1.Longitude as Longitude,
v2.CheckIn as CheckIn2, v2.CheckOut as CheckOut2, v2.Address as Address2, v2.Latitude as Latitude2, v2.Longitude as Longitude2
from visit v1 
inner join visit v2 
	ON v2.latitude2Decimal = v1.latitude2Decimal
	AND v2.longitude2Decimal = v1.longitude2Decimal
	AND v2.userid <> v1.userid
	AND v2.AtRisk = 1
where v1.userid = ?
AND v1.CheckIn > ?
AND (v2.CheckIn >= v1.CheckIn AND V2.CheckOut <= v1.CheckOut)
", userId, startDate);
            return result;
        }
    }
}

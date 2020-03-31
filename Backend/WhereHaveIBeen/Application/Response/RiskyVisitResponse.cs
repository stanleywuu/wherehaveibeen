using Application.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Response
{
    public class RiskyVisitResponse
    {
        public int VisitId { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public string Address { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public int DistanceInKm { get; set; }

        public IList<RiskyVisitResponse> LinkedVisits { get; set; }
    }
}

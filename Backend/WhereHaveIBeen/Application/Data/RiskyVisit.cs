using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Data
{
    public class RiskyVisit
    {
        public int VisitId { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public string Address { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public DateTime CheckIn2 { get; set; }

        public DateTime CheckOut2 { get; set; }

        public string Address2 { get; set; }
        public double Latitude2 { get; set; }
        public double Longitude2 { get; set; }

    }
}

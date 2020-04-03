using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Response
{
    public class VisitResponse
    {
        public int VisitId { get; set; }

        public double Longitude { get; set; }

        public double Latitude { get; set; }

        public DateTime CheckIn { get; set; }

        public DateTime? CheckOut { get; set; }

        // If we could get such information from Google Geolocation API
        public string Address { get; set; }

        public string PlaceName { get; set; }

        public int RiskyInteractions { get; set; }

        public bool AtRisk { get; set; }
    }
}

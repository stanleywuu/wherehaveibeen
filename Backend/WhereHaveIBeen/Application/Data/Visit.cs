using SQLite;
using System;

namespace Application.Data
{
    public class Visit
    {
        [PrimaryKey, AutoIncrement]
        public int VisitId { get; set; }

        public double Longitude { get; set; }

        public double Latitude { get; set; }

        public double LatitudeRounded { get; set; }

        public double LongitudeRounded { get; set; }

        public double Latitude2Decimal { get; set; }

        public double Longitude2Decimal { get; set; }


        public DateTime CheckIn { get; set; }

        public DateTime? CheckOut { get; set; }

        public int UserId { get; set; }

        // If we could get such information from Google Geolocation API
        public string Address { get; set; }

        // If we could get a placeId from google place api
        public string PlaceId { get; set; }

        public string PlaceName { get; set; }

        public bool AtRisk { get; set; }

        public bool CouldBeRisky { get; set; }
    }
}

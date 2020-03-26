using Application.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Requests
{
    public class VisitRequest
    {
        public double Longitude { get; set; }

        public double Latitude { get; set; }

        public DateTime CheckIn { get; set; }

        public DateTime? CheckOut { get; set; }

        public int UserId { get; set; }

        public Visit ToPersistedData()
        {
            return new Visit()
            {
                UserId = UserId,
                 CheckIn = CheckIn,
                 CheckOut = CheckOut,
                 Longitude = Longitude,
                 Latitude = Latitude
            };
        }
    }
}

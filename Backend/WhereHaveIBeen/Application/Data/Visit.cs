using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Data
{
    public class Visit
    {
        [PrimaryKey, AutoIncrement]
        public int VisitId { get; set; }

        public double Longitude { get; set; }

        public double Latitude { get; set; }

        public DateTime CheckIn { get; set; }

        public DateTime? CheckOut { get; set; }

        public int UserId { get; set; }
    }
}

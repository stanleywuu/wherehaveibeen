using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Data
{
    public class PersonAtRisk
    {
        [PrimaryKey, AutoIncrement]
        public int NotificationId { get; set; }

        public int UserId { get; set; }

        public DateTime? SeenNotification { get; set; }

        public DateTime? SentNotification { get; set; }

        public DateTime WarningIssuedDate { get; set; }
    }
}

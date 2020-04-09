using Dapper.Contrib.Extensions;
using System;

namespace Application.Data
{
    [Table("PeopleAtRisk")]
    public class PersonAtRisk
    {
        [Key]
        public int NotificationId { get; set; }

        public int UserId { get; set; }

        public DateTime? SeenNotification { get; set; }

        public DateTime? SentNotification { get; set; }

        public DateTime WarningIssuedDate { get; set; }
    }
}

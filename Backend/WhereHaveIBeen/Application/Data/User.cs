using Dapper.Contrib.Extensions;
using System;

namespace Application.Data
{
    [Table("Users")]
    public class User
    {
        public User()
        {
        }

        [Key]
        public int UserId { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string Token { get; set; }

        public DateTime? TokenExpiry { get; set; }

        public bool AtRisk { get; set; }
    }
}

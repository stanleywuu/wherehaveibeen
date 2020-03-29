using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Data
{
    public class User
    {
        public User()
        {
        }

        [PrimaryKey, AutoIncrement]
        public int UserId { get; set; }

        [Unique]
        public string Username { get; set; }

        [Unique]
        public string Email { get; set; }

        public string Password { get; set; }

        public string Token { get; set; }

        public DateTime? TokenExpiry { get; set; }

        public bool AtRisk { get; set; }
    }
}

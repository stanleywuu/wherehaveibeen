using Application.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Requests
{
    public class UserRequest
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public User ToPersistedData()
        {
            return new User()
            {
                Username = Username,
                Password = Password.Encrypt(),
                Email = Email
            };
        }
    }
}

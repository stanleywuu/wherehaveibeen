using System;

namespace Application.Response
{
    public class TokenResponse
    {
        public int UserId { get; set; }

        public string Token { get; set; }

        public DateTime? Expires { get; set; }

        public bool HasUnseenNotification { get; set; }

        public bool Reported { get; set; }
    }
}

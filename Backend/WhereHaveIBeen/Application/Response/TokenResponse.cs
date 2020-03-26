using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Response
{
    public class TokenResponse
    {
        public string Token { get; set; }

        public DateTime? Expires { get; set; }
    }
}

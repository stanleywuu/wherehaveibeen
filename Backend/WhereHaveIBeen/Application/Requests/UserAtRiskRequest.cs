using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Requests
{
    public class UserAtRiskRequest
    {
        public int UserId { get; set; }

        public bool IsAtRisk { get; set; } = true;
    }
}

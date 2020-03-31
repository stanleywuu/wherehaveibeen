using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Application
{
    public class UserUtilities
    {
        public static int GetUserId(ClaimsPrincipal claims)
        {
            var claim = claims.FindFirst(ClaimTypes.NameIdentifier);

            if (int.TryParse(claim.Value, out int claimUserId))
            {
                return claimUserId;
            }

            return -1;
        }
    }
}

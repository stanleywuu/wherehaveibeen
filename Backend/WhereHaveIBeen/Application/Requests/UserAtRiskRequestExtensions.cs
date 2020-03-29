using Application.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Requests
{
    public static class UserAtRiskRequestExtensions
    {
        public static User ToPersistedData(this UserAtRiskRequest request, User data)
        {
            data.AtRisk = request.IsAtRisk;
            return data;
        }
    }
}

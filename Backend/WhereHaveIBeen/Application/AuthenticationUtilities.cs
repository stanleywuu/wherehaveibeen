using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application
{
    public class AuthenticationUtilities
    {
        public static string Signingkey { set; get; }
        public static string Issuer { set; get; }
        public static string Audience { set; get; }

        public static SecurityKey GetIssuerKey()
        {
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Signingkey));
        }

        public static string GenerateToken(int userId, string username, DateTime expiry)
        {
            var claims = new[]
             {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
             };

            var key = GetIssuerKey();
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: Issuer,
                audience: Audience,
                claims: claims,
                expires: expiry,
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HangOut.Domain.Entities;
using HangOut.Domain.Payload.Settings;
using Microsoft.IdentityModel.Tokens;

namespace HangOut.API.Common.Utils;

public static class JwtUtil
{
    public static string GenerateJwtToken(Account account, JwtSettings jwtSettings)
    {
        var tokenhandler = new JwtSecurityTokenHandler();
        var tokenkey = Encoding.UTF8.GetBytes(jwtSettings.SecurityKey);
        var timeExpire = DateTime.UtcNow.AddDays((double) jwtSettings.TokenExpiry);
        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(
                new Claim[]
                {
                    new Claim("AccountId", account.Id.ToString()),
                    new Claim("Email", account.Email ?? string.Empty),
                    new Claim(ClaimTypes.Role, account.Role.ToString()),
                }
            ),
            Expires = timeExpire,
            Issuer = jwtSettings.Issuer,
            Audience = jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenkey), SecurityAlgorithms.HmacSha256)
        };
        var token = tokenhandler.CreateToken(tokenDescriptor);
        var tokenString = tokenhandler.WriteToken(token);
        return tokenString;
    }
}
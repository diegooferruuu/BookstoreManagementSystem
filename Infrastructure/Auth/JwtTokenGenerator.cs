using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BookstoreManagementSystem.Application.Interfaces;
using BookstoreManagementSystem.Domain.Models;
using Microsoft.IdentityModel.Tokens;

namespace BookstoreManagementSystem.Infrastructure.Auth
{
    public class JwtTokenGenerator : ITokenGenerator
    {
        public string CreateToken(User user, IEnumerable<string> roles, DateTimeOffset now, JwtOptions options)
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(JwtRegisteredClaimNames.UniqueName, user.Username),
                new(JwtRegisteredClaimNames.Email, user.Email),
                new(ClaimTypes.Name, user.Username),
                new("given_name", user.FirstName ?? string.Empty),
                new("family_name", user.LastName ?? string.Empty),
                new("middle_name", user.MiddleName ?? string.Empty),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            foreach (var r in roles)
                claims.Add(new Claim(ClaimTypes.Role, r));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: options.Issuer,
                audience: options.Audience,
                claims: claims,
                notBefore: now.UtcDateTime,
                expires: now.AddMinutes(options.ExpiresMinutes).UtcDateTime,
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

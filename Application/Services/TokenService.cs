using Application.Services.Interfaces;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;

        public TokenService(IConfiguration config)
        {
            _config = config;
        }

        public string CreateToken(Employee employee, IList<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, employee.Id.ToString()),
                new Claim(ClaimTypes.Name, employee.FullName),
                new Claim(ClaimTypes.Email, employee.Email)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return GenerateToken(claims);
        }

        public string CreateToken(Intern student, IList<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, student.Id.ToString()),
                new Claim(ClaimTypes.Name, student.FullName)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return GenerateToken(claims);
        }

        private string GenerateToken(List<Claim> claims)
        {
            var jwtKey = _config["Jwt:Key"];

            if (string.IsNullOrEmpty(jwtKey))
            {
                throw new InvalidOperationException("Jwt:Key is not configured in appsettings.json");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var issuer = _config["Jwt:Issuer"] ?? throw new InvalidOperationException("Jwt:Issuer is missing");
            var audience = _config["Jwt:Audience"] ?? throw new InvalidOperationException("Jwt:Audience is missing");
            var expiresMinutes = _config["Jwt:ExpiresMinutes"];
            var expiresTime = string.IsNullOrEmpty(expiresMinutes) ? 60 : Convert.ToDouble(expiresMinutes);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiresTime),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

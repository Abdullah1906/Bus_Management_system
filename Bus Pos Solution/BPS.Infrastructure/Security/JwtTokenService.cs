using BPS.Application.Interfaces;
using BPS.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BPS.Infrastructure.Security
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _configuration;

        public JwtTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(User user)
        {
            var key =
                _configuration["Jwt:Key"]
                ?? throw new InvalidOperationException(
                    "JWT Key is not configured.");

            var issuer =
                _configuration["Jwt:Issuer"];

            var audience =
                _configuration["Jwt:Audience"];

            var expirationMinutes =
                int.Parse(
                    _configuration["Jwt:ExpirationMinutes"]
                    ?? "60");

            var claims = new List<Claim>
            {
                   new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),

                new(JwtRegisteredClaimNames.Sub,
                    user.Id.ToString()),

                new(JwtRegisteredClaimNames.UniqueName,
                    user.Username),

                new(ClaimTypes.Name,
                    user.Username),

                new(ClaimTypes.Email,
                    user.Email),

                new(ClaimTypes.Role,
                    user.Role)
            };

            var securityKey =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(key));

            var credentials =
                new SigningCredentials(
                    securityKey,
                    SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    expirationMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }
    }
}

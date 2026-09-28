using BPS.Application.Interfaces;
using BPS.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
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

        //public string GenerateToken(User user)
        //{
        //    var key =
        //        _configuration["Jwt:Key"]
        //        ?? throw new InvalidOperationException(
        //            "JWT Key is not configured.");

        //    var issuer =
        //        _configuration["Jwt:Issuer"];

        //    var audience =
        //        _configuration["Jwt:Audience"];

        //    var expirationMinutes =
        //        int.Parse(
        //            _configuration["Jwt:ExpirationMinutes"]
        //            ?? "60");

        //    var claims = new List<Claim>
        //    {
        //           new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),

        //        new(JwtRegisteredClaimNames.Sub,
        //            user.Id.ToString()),

        //        new(JwtRegisteredClaimNames.UniqueName,
        //            user.Username),

        //        new(ClaimTypes.Name,
        //            user.Username),

        //        new(ClaimTypes.Email,
        //            user.Email),

        //        new(ClaimTypes.Role,
        //            user.Role)
        //    };

        //    var securityKey =
        //        new SymmetricSecurityKey(
        //            Encoding.UTF8.GetBytes(key));

        //    var credentials =
        //        new SigningCredentials(
        //            securityKey,
        //            SecurityAlgorithms.HmacSha256);

        //    var token = new JwtSecurityToken(
        //        issuer: issuer,
        //        audience: audience,
        //        claims: claims,
        //        expires: DateTime.UtcNow.AddMinutes(
        //            expirationMinutes),
        //        signingCredentials: credentials);

        //    return new JwtSecurityTokenHandler()
        //        .WriteToken(token);
        //}

        public string GenerateAccessToken(User user)
        {
            var key = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is not configured.");
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];
            var expirationMinutes = int.Parse(_configuration["Jwt:ExpirationMinutes"] ?? "15");

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(JwtRegisteredClaimNames.UniqueName, user.Username),
                new(ClaimTypes.Name, user.Username),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Role, user.Role)
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            var key = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is not configured.");

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidAudience = _configuration["Jwt:Audience"],
                ValidateIssuer = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
                if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                    !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    return null;
                }
                return principal;
            }
            catch
            {
                return null;
            }
        }
    
}
}

﻿using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ShopListApp.Interfaces;
using ShopListApp.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ShopListApp.Managers
{
    public class JwtTokenManager : ITokenManager
    {
        private readonly IConfiguration _config;
        public JwtTokenManager(IConfiguration config)
        {
            _config = config;
        }
        public string GenerateIdentityToken(User user)
        {
            var tokenConfig = _config.GetSection("TokenConfiguration");
            string issuer = "https://localhost:7101";
            string audience = "https://localhost:7101";
            string secretKey = Environment.GetEnvironmentVariable("SecretJwtKey") 
                ?? tokenConfig.GetValue<string>("SecretKey") 
                ?? String.Empty;
            int jwtExpireMinutes = tokenConfig.GetValue<int>("AccessTokenExpirationMinutes");
            DateTime jwtExpireDate = DateTime.Now.AddMinutes(jwtExpireMinutes);
            var symmKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(symmKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,   
                claims: claims, 
                expires: jwtExpireDate, 
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateHashRefreshToken()
        {
            byte[] refreshToken = RandomNumberGenerator.GetBytes(64);
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(refreshToken);
                return Convert.ToBase64String(bytes);
            }
        }

        public string GenerateRefreshToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        public int GetRefreshTokenExpirationDays() => _config.GetValue<int>("TokenConfiguration:RefreshTokenExpirationDays");

        public string? GetHashRefreshToken(string refreshToken)
        {
            try
            {
                byte[] bytes = Convert.FromBase64String(refreshToken);
                using (var sha256 = SHA256.Create())
                {
                    var hash = sha256.ComputeHash(bytes);
                    return Convert.ToBase64String(hash);
                }
            }
            catch
            {
                return null;
            }
        }
    }
}

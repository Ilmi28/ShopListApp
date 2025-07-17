using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ShopListApp.Core.Dtos;
using ShopListApp.Core.Interfaces.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ShopListApp.Infrastructure.TokenManagers;

public class JwtTokenManager(IConfiguration config) : ITokenManager
{
    public string GenerateAccessToken(UserDto user)
    {
        var tokenConfig = config.GetSection("TokenConfiguration");
        string issuer = tokenConfig.GetSection("Issuer").Get<string>() ?? string.Empty;
        string[] audience = tokenConfig.GetSection("Audience").Get<string[]>() ?? [];
        string secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY") 
            ?? string.Empty;
        int jwtExpireMinutes = tokenConfig.GetValue<int>("AccessTokenExpirationMinutes");
        DateTime jwtExpireDate = DateTime.Now.AddMinutes(jwtExpireMinutes);
        var symmKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(symmKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
        };

        foreach (var aud in audience)
            claims.Add(new Claim("aud", aud));

        var token = new JwtSecurityToken(
            issuer: issuer,
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

    public int GetRefreshTokenExpirationDays() => config.GetValue<int>("TokenConfiguration:RefreshTokenExpirationDays");
    public int GetAccessTokenExpirationMinutes() => config.GetValue<int>("TokenConfiguration:AccessTokenExpirationMinutes");

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

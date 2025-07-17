using ShopListApp.Core.Dtos;

namespace ShopListApp.Core.Interfaces.Identity;

public interface ITokenManager
{
    string GenerateAccessToken(UserDto user);
    int GetRefreshTokenExpirationDays();
    int GetAccessTokenExpirationMinutes();
    string? GetHashRefreshToken(string refreshToken);
    string GenerateRefreshToken();
}

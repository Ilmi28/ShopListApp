using ShopListApp.Core.Dtos;

namespace ShopListApp.Core.Interfaces.Identity;

public interface ITokenManager
{
    string GenerateAccessToken(UserDto user);
    int GetRefreshTokenExpirationDays();
    string? GetHashRefreshToken(string refreshToken);
    string GenerateRefreshToken();
}

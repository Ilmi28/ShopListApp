using ShopListApp.Models;

namespace ShopListApp.Interfaces
{
    public interface ITokenManager
    {
        string GenerateAccessToken(User user);
        int GetRefreshTokenExpirationDays();
        string? GetHashRefreshToken(string refreshToken);
        string GenerateRefreshToken();
    }
}

using ShopListApp.Models;

namespace ShopListApp.Interfaces
{
    public interface ITokenManager
    {
        string GenerateIdentityToken(User user);
        int GetRefreshTokenExpirationDays();
        string? GetHashRefreshToken(string refreshToken);
        string GenerateRefreshToken();
    }
}

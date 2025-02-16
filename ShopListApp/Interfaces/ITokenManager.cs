using ShopListApp.Models;

namespace ShopListApp.Interfaces
{
    public interface ITokenManager
    {
        string GenerateIdentityToken(User user);
        string GenerateHashRefreshToken();
        int GetRefreshTokenExpirationDays();
        string GetHashRefreshToken(string refreshToken);
    }
}

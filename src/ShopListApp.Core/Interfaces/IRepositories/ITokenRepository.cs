using ShopListApp.Core.Models;

namespace ShopListApp.Core.Interfaces.IRepositories
{
    public interface ITokenRepository
    {
        Task<bool> AddToken(Token token);
        Task<Token?> GetToken(string refreshToken);
    }
}

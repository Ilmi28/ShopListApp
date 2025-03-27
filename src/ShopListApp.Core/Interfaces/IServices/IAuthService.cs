using ShopListApp.Commands;
using ShopListApp.Core.Commands.Auth;

namespace ShopListApp.Interfaces.IServices
{
    public interface IAuthService
    {
        Task<(string identityToken, string refreshToken)> RegisterUser(CreateUserCommand cmd);
        Task<(string identityToken, string refreshToken)> LoginUser(LoginUserCommand cmd);
        Task<string> RefreshAccessToken(RefreshTokenCommand cmd);
    }
}

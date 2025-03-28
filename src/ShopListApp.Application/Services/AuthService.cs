using ShopListApp.Commands;
using ShopListApp.Core.Commands.Auth;
using ShopListApp.Core.Dtos;
using ShopListApp.Core.Interfaces;
using ShopListApp.Core.Interfaces.Identity;
using ShopListApp.Core.Interfaces.ILogger;
using ShopListApp.Core.Responses;
using ShopListApp.Enums;
using ShopListApp.Exceptions;
using ShopListApp.Interfaces;
using ShopListApp.Interfaces.IServices;
using ShopListApp.Models;

namespace ShopListApp.Services
{
    public class AuthService : IAuthService
    {
        private IUserManager _userManager;
        private IDbLogger<UserDto> _logger;
        private ITokenManager _tokenManager;
        private ITokenRepository _tokenRepository;
        public AuthService(IUserManager userManager, 
            IDbLogger<UserDto> logger,
            ITokenManager tokenManager, 
            ITokenRepository tokenRepository)
        {
            _userManager = userManager;
            _logger = logger;
            _tokenManager = tokenManager;
            _tokenRepository = tokenRepository;
        }

        public async Task<LoginRegisterResponse> RegisterUser(RegisterUserCommand cmd)
        {
            _ = cmd ?? throw new ArgumentNullException(nameof(cmd));
            var user = new UserDto
            {
                Id = Guid.NewGuid().ToString(),
                UserName = cmd.UserName,
                Email = cmd.Email,
            };
            try
            {
                var userByEmail = await _userManager.FindByEmailAsync(cmd.Email);
                var userByName = await _userManager.FindByNameAsync(cmd.UserName);
                if (userByEmail != null)
                    throw new UserWithEmailAlreadyExistsException();
                if (userByName != null)
                    throw new UserWithUserNameAlreadyExistsException();
                await _userManager.CreateAsync(user, cmd.Password);
                string identityToken = _tokenManager.GenerateAccessToken(user);
                string refreshToken = _tokenManager.GenerateRefreshToken();
                string hashRefreshToken = _tokenManager.GetHashRefreshToken(refreshToken)
                    ?? throw new DatabaseErrorException();
                await CreateRefreshTokenInDb(hashRefreshToken, user);
                await _logger.Log(Operation.Register, user);
                return new LoginRegisterResponse { IdentityToken = identityToken, RefreshToken = refreshToken };
            }
            catch (UserAlreadyExistsException)
            {
                throw;
            }
            catch
            {
                throw new DatabaseErrorException();
            }
        }

        private async Task CreateRefreshTokenInDb(string refreshToken, UserDto user)
        {
            var token = new Token
            {
                UserId = user.Id,
                RefreshTokenHash = refreshToken,
                ExpirationDate = DateTime.Now.AddDays(_tokenManager.GetRefreshTokenExpirationDays())
            };
            var result = await _tokenRepository.AddToken(token);
            if (!result)
                throw new DatabaseErrorException();
        }

        public async Task<LoginRegisterResponse> LoginUser(LoginUserCommand cmd)
        {
            _ = cmd ?? throw new ArgumentNullException(nameof(cmd));
            try
            {
                var user = await _userManager.FindByEmailAsync(cmd.UserIdentifier);
                if (user == null)
                    user = await _userManager.FindByNameAsync(cmd.UserIdentifier);
                if (user == null)
                    throw new UnauthorizedAccessException();
                var result = await _userManager.CheckPasswordAsync(user, cmd.Password);
                if (!result)
                    throw new UnauthorizedAccessException();
                string identityToken = _tokenManager.GenerateAccessToken(user);
                string refreshToken = _tokenManager.GenerateRefreshToken();
                string hashRefreshToken = _tokenManager.GetHashRefreshToken(refreshToken) 
                    ?? throw new UnauthorizedAccessException();
                await CreateRefreshTokenInDb(hashRefreshToken, user);
                await _logger.Log(Operation.Login, user);
                return new LoginRegisterResponse { IdentityToken = identityToken, RefreshToken = refreshToken };
            }
            catch (UnauthorizedAccessException)
            {
                throw;
            }
            catch
            {
                throw new DatabaseErrorException();
            }
        }

        public async Task<string> RefreshAccessToken(RefreshTokenCommand cmd)
        {
            _ = cmd ?? throw new ArgumentNullException(nameof(cmd));
            try
            {
                string hash = _tokenManager.GetHashRefreshToken(cmd.RefreshToken) 
                                                        ?? throw new UnauthorizedAccessException();
                var token = await _tokenRepository.GetToken(hash)
                                                        ?? throw new UnauthorizedAccessException();
                var user = await _userManager.FindByIdAsync(token.UserId)
                                                        ?? throw new UnauthorizedAccessException();
                var identityToken = _tokenManager.GenerateAccessToken(user);
                return identityToken;
            }
            catch (UnauthorizedAccessException)
            {
                throw;
            }
            catch
            {
                throw new DatabaseErrorException();
            }
        }
    }
}

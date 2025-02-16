using Microsoft.AspNetCore.Identity;
using ShopListApp.Commands;
using ShopListApp.Enums;
using ShopListApp.Exceptions;
using ShopListApp.Interfaces;
using ShopListApp.Managers;
using ShopListApp.Models;
using ShopListApp.Repositories;

namespace ShopListApp.Services
{
    public class AuthService : IAuthService
    {
        private UserManager<User> _userManager;
        private IDbLogger<User> _logger;
        private ITokenManager _tokenManager;
        private ITokenRepository _tokenRepository;
        public AuthService(UserManager<User> userManager, 
            IDbLogger<User> logger,
            ITokenManager tokenManager, 
            ITokenRepository tokenRepository)
        {
            _userManager = userManager;
            _logger = logger;
            _tokenManager = tokenManager;
            _tokenRepository = tokenRepository;
        }

        public async Task<(string identityToken, string refreshToken)> RegisterUser(CreateUserCommand cmd)
        {
            _ = cmd ?? throw new ArgumentNullException(nameof(cmd));
            var user = new User
            {
                UserName = cmd.UserName,
                Email = cmd.Email,
            };
            try
            {
                var userByEmail = await _userManager.FindByEmailAsync(cmd.Email);
                var userByName = await _userManager.FindByNameAsync(cmd.UserName);
                if (userByEmail != null)
                    throw new UserAlreadyExistsException("User with this email already exists");
                if (userByName != null)
                    throw new UserAlreadyExistsException("User with this username already exists");
                await _userManager.CreateAsync(user, cmd.Password);
                string identityToken = _tokenManager.GenerateIdentityToken(user);
                string refreshToken = _tokenManager.GenerateHashRefreshToken();
                await CreateRefreshToken(refreshToken, user);
                await _logger.Log(Operation.Register, user);
                return (identityToken, refreshToken);
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

        private async Task CreateRefreshToken(string refreshToken, User user)
        {
            var token = new Token
            {
                UserId = user.Id,
                RefreshTokenHash = refreshToken,
                User = user,
                ExpirationDate = DateTime.Now.AddDays(_tokenManager.GetRefreshTokenExpirationDays())
            };
            var result = await _tokenRepository.AddToken(token);
            if (!result)
                throw new DatabaseErrorException();
        }

        public async Task<(string, string)> LoginUser(LoginUserCommand cmd)
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
                string identityToken = _tokenManager.GenerateIdentityToken(user);
                string refreshToken = _tokenManager.GenerateHashRefreshToken();
                await CreateRefreshToken(refreshToken, user);
                await _logger.Log(Operation.Login, user);
                return (identityToken, refreshToken);
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
                string hash = _tokenManager.GetHashRefreshToken(cmd.RefreshToken);
                var token = await _tokenRepository.GetToken(hash);
                if (token == null)
                    throw new UnauthorizedAccessException();
                var user = await _userManager.FindByIdAsync(token.UserId);
                if (user == null)
                    throw new UnauthorizedAccessException();
                var identityToken = _tokenManager.GenerateIdentityToken(user);
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

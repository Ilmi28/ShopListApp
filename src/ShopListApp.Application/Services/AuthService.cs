﻿using ShopListApp.Core.Commands.Auth;
using ShopListApp.Core.Dtos;
using ShopListApp.Core.Enums;
using ShopListApp.Core.Exceptions;
using ShopListApp.Core.Interfaces.Identity;
using ShopListApp.Core.Interfaces.ILogger;
using ShopListApp.Core.Interfaces.IRepositories;
using ShopListApp.Core.Interfaces.IServices;
using ShopListApp.Core.Models;
using ShopListApp.Core.Responses;

namespace ShopListApp.Application.Services;

public class AuthService(IUserManager userManager,
    IDbLogger<UserDto> logger,
    ITokenManager tokenManager,
    ITokenRepository tokenRepository) : IAuthService
{
    public async Task<LoginRegisterResponse> RegisterUser(RegisterUserCommand cmd)
    {
        _ = cmd ?? throw new ArgumentNullException(nameof(cmd));
        var user = new UserDto
        {
            Id = Guid.NewGuid().ToString(),
            UserName = cmd.UserName,
            Email = cmd.Email,
        };
        var userByEmail = await userManager.FindByEmailAsync(cmd.Email);
        var userByName = await userManager.FindByNameAsync(cmd.UserName);
        if (userByEmail != null)
            throw new UserWithEmailAlreadyExistsException($"User with email {cmd.Email} already exists");
        if (userByName != null)
            throw new UserWithUserNameAlreadyExistsException($"User with username {cmd.UserName} already exists");
        await userManager.CreateAsync(user, cmd.Password);
        string identityToken = tokenManager.GenerateAccessToken(user);
        string refreshToken = tokenManager.GenerateRefreshToken();
        string hashRefreshToken = tokenManager.GetHashRefreshToken(refreshToken) ??
            throw new Exception("Unexpected error occured when trying to create hash of refresh token");
        await CreateRefreshTokenInDb(hashRefreshToken, user);
        await logger.Log(Operation.Register, user);
        return new LoginRegisterResponse { IdentityToken = identityToken, RefreshToken = refreshToken };
    }

    private async Task CreateRefreshTokenInDb(string refreshToken, UserDto user)
    {
        var token = new Token
        {
            UserId = user.Id,
            RefreshTokenHash = refreshToken,
            ExpirationDate = DateTime.Now.AddDays(tokenManager.GetRefreshTokenExpirationDays())
        };
        var result = await tokenRepository.AddToken(token);
        if (!result)
            throw new Exception("Unexpected error occured when adding token");
    }

    public async Task<LoginRegisterResponse> LoginUser(LoginUserCommand cmd)
    {
        _ = cmd ?? throw new ArgumentNullException(nameof(cmd));
        var user = await userManager.FindByEmailAsync(cmd.UserIdentifier);
        if (user == null)
            user = await userManager.FindByNameAsync(cmd.UserIdentifier);
        if (user == null)
            throw new UnauthorizedAccessException();
        var result = await userManager.CheckPasswordAsync(user, cmd.Password);
        if (!result)
            throw new UnauthorizedAccessException();
        string identityToken = tokenManager.GenerateAccessToken(user);
        string refreshToken = tokenManager.GenerateRefreshToken();
        string hashRefreshToken = tokenManager.GetHashRefreshToken(refreshToken)
            ?? throw new UnauthorizedAccessException();
        await CreateRefreshTokenInDb(hashRefreshToken, user);
        await logger.Log(Operation.Login, user);
        return new LoginRegisterResponse { IdentityToken = identityToken, RefreshToken = refreshToken };
    }

    public async Task<string> RefreshAccessToken(RefreshTokenCommand cmd)
    {
        _ = cmd ?? throw new ArgumentNullException(nameof(cmd));
        string hash = tokenManager.GetHashRefreshToken(cmd.RefreshToken)
                                                    ?? throw new UnauthorizedAccessException();
        var token = await tokenRepository.GetToken(hash)
                                                ?? throw new UnauthorizedAccessException();
        var user = await userManager.FindByIdAsync(token.UserId)
                                                ?? throw new UnauthorizedAccessException();
        var identityToken = tokenManager.GenerateAccessToken(user);
        return identityToken;
    }
}

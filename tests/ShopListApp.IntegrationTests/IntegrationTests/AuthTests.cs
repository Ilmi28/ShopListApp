﻿using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using ShopListApp.Core.Commands.Auth;
using ShopListApp.Core.Interfaces.Identity;
using ShopListApp.Core.Models;
using ShopListApp.Infrastructure.Database.Context;
using ShopListApp.Infrastructure.Database.Identity.AppUser;
using ShopListApp.IntegrationTests.IntegrationTests.WebApplicationFactories;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace ShopListApp.IntegrationTests.IntegrationTests;

public class AuthTests : IClassFixture<AuthWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly ShopListDbContext _context;
    private readonly UserManager<User> _manager;
    private readonly ITokenManager _tokenManager;
    public AuthTests(AuthWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        var scope = factory.Services.CreateScope();
        _context = scope.ServiceProvider.GetRequiredService<ShopListDbContext>();
        _manager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        _tokenManager = scope.ServiceProvider.GetRequiredService<ITokenManager>();
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();
        SeedData();
    }

    private void SeedData()
    {
        var user = new User
        {
            Id = "123",
            UserName = "test",
            Email = "test@gmail.com"
        };

        var token = new Token
        {
            Id = 1,
            RefreshTokenHash = _tokenManager.GetHashRefreshToken("Xj4z8x+7Q0A=")!,
            ExpirationDate = DateTime.Now.AddDays(1),
            UserId = user.Id
        };
        var token1 = new Token
        {
            Id = 2,
            RefreshTokenHash = _tokenManager.GetHashRefreshToken("K3N5TzFhMkM=")!,
            ExpirationDate = DateTime.Now.AddDays(-1),
            UserId = user.Id
        };
        var token2 = new Token
        {
            Id = 3,
            RefreshTokenHash = _tokenManager.GetHashRefreshToken("T1hKQ1VmcDg=")!,
            ExpirationDate = DateTime.Now.AddDays(1),
            UserId = user.Id,
            IsRevoked = true
        };
        _manager.CreateAsync(user, "Password123@").Wait();
        _context.Tokens.Add(token);
        _context.Tokens.Add(token1);
        _context.Tokens.Add(token2);
        _context.SaveChanges();
    }

    [Fact]
    public async Task RegisterUser_ValidUser_ReturnsNoContent()
    {
        var cmd = new RegisterUserCommand
        {
            UserName = "test1",
            Email = "test1@gmail.com",
            Password = "Password123@"
        };
        
        var response = await _client.PostAsJsonAsync("/api/auth/register", cmd);
        response.Headers.TryGetValues("Set-Cookie", out var cookieValues);
        var accessToken = cookieValues.Where(v => v.StartsWith("accessToken=")).FirstOrDefault();
        var refreshToken = cookieValues.Where(v => v.StartsWith("refreshToken=")).FirstOrDefault(); 
        int userCount = _context.Users.Count();
        int tokenCount = _context.Tokens.Count();
        int userLogsCount = _context.UserLogs.Count();

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.True(userCount == 2);
        Assert.True(tokenCount == 4);
        Assert.True(userLogsCount == 1);
        Assert.False(string.IsNullOrEmpty(accessToken));
        Assert.False(string.IsNullOrEmpty(refreshToken));
    }

    [Theory]
    [InlineData("test", "test1@gmail.com")]
    [InlineData("test1", "test@gmail.com")]
    public async Task RegisterUser_UserAlreadyExists_ReturnsBadRequest(string username, string email)
    {
        var cmd = new RegisterUserCommand
        {
            UserName = username,
            Email = email,
            Password = "Password123@"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/register", cmd);
        var errorMessages = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task RegisterUser_NullRequest_ReturnsBadRequest()
    {
        RegisterUserCommand? cmd = null;

        var response = await _client.PostAsJsonAsync("/api/auth/register", cmd);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData("a", "test1@gmail.com", "Password123@")]
    [InlineData("test1", "test1gmail.com", "Password123@")]
    [InlineData("test1", "test1@gmail.com", "password")]
    public async Task RegisterUser_InvalidInput_ReturnsBadRequest(string username, string email, string password)
    {
        RegisterUserCommand cmd = new()
        {
            UserName = username,
            Email = email,
            Password = password
        };

        var response = await _client.PostAsJsonAsync("/api/auth/register", cmd);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData("test", "Password123@")]
    [InlineData("test@gmail.com", "Password123@")]
    public async Task LoginUser_ValidInput_ReturnsNoContent(string userIdentifier, string password)
    {
        LoginUserCommand cmd = new()
        {
            UserIdentifier = userIdentifier,
            Password = password
        };

        var response = await _client.PostAsJsonAsync("/api/auth/login", cmd);
        response.Headers.TryGetValues("Set-Cookie", out var cookieValues);
        var accessToken = cookieValues.Where(v => v.StartsWith("accessToken=")).ToString();
        var refreshToken = cookieValues.Where(v => v.StartsWith("refreshToken=")).ToString();
        response.EnsureSuccessStatusCode();
        int tokenCount = _context.Tokens.Count();
        int revokedTokenCount = _context.Tokens.Where(x => x.IsRevoked).Count();
        int userLogsCount = _context.UserLogs.Count();

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.True(tokenCount == 4);
        Assert.True(userLogsCount == 1);
        Assert.True(revokedTokenCount == 2);
        Assert.False(string.IsNullOrEmpty(accessToken));
        Assert.False(string.IsNullOrEmpty(refreshToken));
    }

    [Fact]
    public async Task LoginUser_NullRequest_ReturnsBadRequest()
    {
        LoginUserCommand? cmd = null;

        var response = await _client.PostAsJsonAsync("/api/auth/login", cmd);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData("test", "Password123!")]
    [InlineData("test@gmail.com", "Password123!")]
    [InlineData("test1", "Password123@")]
    [InlineData("test1@gmail.com", "Password123@")]
    public async Task LoginUser_InvalidUserIdentifierOrPassword_ReturnsUnauthorized(string userIdentifier, string password)
    {
        LoginUserCommand cmd = new LoginUserCommand()
        {
            UserIdentifier = userIdentifier,
            Password = password
        };

        var response = await _client.PostAsJsonAsync("/api/auth/login", cmd);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task RefreshToken_ValidRequest_ReturnsNoContent()
    {
        RefreshTokenCommand cmd = new RefreshTokenCommand
        {
            RefreshToken = "Xj4z8x+7Q0A="
        };
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/auth/refresh");
        request.Headers.Add("Cookie", $"refreshToken={cmd.RefreshToken};");
        var response = await _client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task RefreshToken_NullRequest_ReturnsUnathorized()
    {
        RefreshTokenCommand? cmd = null;

        var response = await _client.PostAsJsonAsync("/api/auth/refresh", cmd);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("fFdqL3Y2U0g=")]
    [InlineData("K3N5TzFhMkM=")]
    [InlineData("T1hKQ1VmcDg=")]
    public async Task RefreshToken_InvalidRefreshToken_ReturnsUnauthorized(string refreshToken)
    {
        RefreshTokenCommand cmd = new RefreshTokenCommand
        {
            RefreshToken = refreshToken
        };

        var response = await _client.PostAsJsonAsync("/api/auth/refresh", cmd);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }



}

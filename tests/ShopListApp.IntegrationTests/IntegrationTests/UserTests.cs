using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using ShopListApp.Core.Commands.Delete;
using ShopListApp.Core.Commands.Update;
using ShopListApp.Core.Dtos;
using ShopListApp.Core.Interfaces.Identity;
using ShopListApp.Infrastructure.Database.Identity.AppUser;
using ShopListApp.IntegrationTests.IntegrationTests.WebApplicationFactories;
using ShopListApp.TestUtilities.Stubs;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ShopListApp.IntegrationTests.IntegrationTests;

public class UserTests : IClassFixture<UserWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly TestDbContext _context;
    private readonly UserManager<User> _manager;
    private readonly ITokenManager _tokenManager;
    public UserTests(UserWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        var scope = factory.Services.CreateScope();
        _context = scope.ServiceProvider.GetRequiredService<TestDbContext>();
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
            Id = "1",
            UserName = "test",
            Email = "test@gmail.com"
        };
        var otherUser = new User
        {
            Id = "2",
            UserName = "other",
            Email = "test2@gmail.com"
        };
        _manager.CreateAsync(user, "Password123@");
        _manager.CreateAsync(otherUser, "Password123@");
    }

    [Fact]
    public async Task UpdateUser_ValidInput_UpdatesUser()
    {
        var user = await _manager.FindByIdAsync("1");
        string? oldHashPassword = user!.PasswordHash;
        var userDto = new UserDto
        {
            Id = user.Id,
            UserName = user.UserName!,
            Email = user.Email!
        };
        string jwtToken = _tokenManager.GenerateAccessToken(userDto!);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        var cmd = new UpdateUserCommand
        {
            UserName = "newName",
            Email = "new@gmail.com",
            CurrentPassword = "Password123@",
            NewPassword = "NewPassword123@"
        };

        var response = await _client.PutAsJsonAsync("api/user/update", cmd);

        await _context.Entry(user!).ReloadAsync();
        var updatedUser = await _manager.FindByIdAsync(userDto.Id);
        var userLogCount = _context.UserLogs.Count();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("newName", updatedUser!.UserName);
        Assert.Equal("new@gmail.com", updatedUser.Email);
        Assert.NotEqual(oldHashPassword, updatedUser.PasswordHash);
        Assert.Equal(1, userLogCount);
    }

    [Fact]
    public async Task UpdateUser_ProvidedNullValuesAndValidPassword_ReturnsOK()
    {
        var cmd = new UpdateUserCommand
        {
            UserName = null,
            Email = null,
            CurrentPassword = "Password123@",
            NewPassword = null
        };
        var oldUser = await _manager.FindByIdAsync("1");
        var userDto = new UserDto
        {
            Id = oldUser!.Id,
            UserName = oldUser.UserName!,
            Email = oldUser.Email!
        };
        var jwtToken = _tokenManager.GenerateAccessToken(userDto!);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

        var response = await _client.PutAsJsonAsync("api/user/update", cmd);
        await _context.Entry(oldUser!).ReloadAsync();
        var updatedUser = await _manager.FindByIdAsync("1");
        var userLogCount = _context.UserLogs.Count();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(oldUser!.UserName, updatedUser!.UserName);
        Assert.Equal(oldUser.Email, updatedUser.Email);
        Assert.Equal(oldUser.PasswordHash, updatedUser.PasswordHash);
        Assert.Equal(0, userLogCount);

    }

    [Fact]
    public async Task UpdateUser_NullRequest_ReturnsBadRequest()
    {
        UpdateUserCommand? cmd = null;
        var user = await _manager.FindByIdAsync("1");
        var userDto = new UserDto
        {
            Id = user!.Id,
            UserName = user.UserName!,
            Email = user.Email!
        };
        var jwtToken = _tokenManager.GenerateAccessToken(userDto!);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

        var response = await _client.PutAsJsonAsync("api/user/update", cmd);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }


    [Fact]
    public async Task UpdateUser_InvalidCurrentPassword_ReturnsUnauthorized()
    {
        var cmd = new UpdateUserCommand
        {
            UserName = "newName",
            Email = "example@gmail.com",
            CurrentPassword = "InvalidPassword123@",
            NewPassword = "NewPassword123@"
        };

        var user = await _manager.FindByIdAsync("1");
        var userDto = new UserDto
        {
            Id = user!.Id,
            UserName = user.UserName!,
            Email = user.Email!
        };
        var jwtToken = _tokenManager.GenerateAccessToken(userDto!);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

        var response = await _client.PutAsJsonAsync("api/user/update", cmd);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }


    [Theory]
    [InlineData("a", null, null)]
    [InlineData(null, "example!gmail.com", null)]
    [InlineData(null, null, "12345")]
    public async Task UpdateUser_InvalidInput_ReturnsBadRequest(string? username, string? email, string? password)
    {
        var cmd = new UpdateUserCommand
        {
            UserName = username,
            Email = email,
            CurrentPassword = "Password123@",
            NewPassword = password
        };
        var user = await _manager.FindByIdAsync("1");
        var userDto = new UserDto
        {
            Id = user!.Id,
            UserName = user.UserName!,
            Email = user.Email!
        };
        var jwtToken = _tokenManager.GenerateAccessToken(userDto!);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

        var response = await _client.PutAsJsonAsync("api/user/update", cmd);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("invalidToken")]
    public async Task UpdateUser_InvalidToken_ReturnsUnauthorized(string? token)
    {
        var cmd = new UpdateUserCommand
        {
            UserName = "newName",
            Email = "example@gmail.com",
            CurrentPassword = "Password123@",
            NewPassword = "NewPassword123@"
        };
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.PutAsJsonAsync("api/user/update", cmd);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DeleteUser_ValidPassword_ReturnsOK()
    {
        var cmd = new DeleteUserCommand
        {
            Password = "Password123@"
        };
        var user = await _manager.FindByIdAsync("1");
        var userDto = new UserDto
        {
            Id = user!.Id,
            UserName = user.UserName!,
            Email = user.Email!
        };
        var jwtToken = _tokenManager.GenerateAccessToken(userDto!);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Delete,
            RequestUri = new Uri("api/user/delete", UriKind.Relative),
            Content = JsonContent.Create(cmd)
        };
        var response = await _client.SendAsync(request);

        var  dbUserCount = _context.Users.Count();
        var userLogCount = _context.UserLogs.Count();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(1, dbUserCount);
        Assert.Equal(1, userLogCount);
    }

    [Fact]
    public async Task DeleteUser_InvalidPassword_ReturnsUnauthorized()
    {
        var cmd = new DeleteUserCommand
        {
            Password = "InvalidPassword123@"
        };
        var user = await _manager.FindByIdAsync("1");
        var userDto = new UserDto
        {
            Id = user!.Id,
            UserName = user.UserName!,
            Email = user.Email!
        };
        var jwtToken = _tokenManager.GenerateAccessToken(userDto!);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Delete,
            RequestUri = new Uri("api/user/delete", UriKind.Relative),
            Content = JsonContent.Create(cmd)
        };
        var response = await _client.SendAsync(request);
        var dbUserCount = _context.Users.Count();
        var userLogCount = _context.UserLogs.Count();
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal(2, dbUserCount);
        Assert.Equal(0, userLogCount);
    }

    [Fact]
    public async Task DeleteUser_NullRequest_ReturnsBadRequest()
    {
        DeleteUserCommand? cmd = null;
        var user = await _manager.FindByIdAsync("1");
        var userDto = new UserDto
        {
            Id = user!.Id,
            UserName = user.UserName!,
            Email = user.Email!
        };
        var jwtToken = _tokenManager.GenerateAccessToken(userDto!);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Delete,
            RequestUri = new Uri("api/user/delete", UriKind.Relative),
            Content = JsonContent.Create(cmd)
        };
        var response = await _client.SendAsync(request);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("InvalidToken")]
    public async Task DeleteUser_InvalidToken_ReturnsUnauthorized(string? token)
    {
        var cmd = new DeleteUserCommand
        {
            Password = "Password123@"
        };
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Delete,
            RequestUri = new Uri("api/user/delete", UriKind.Relative),
            Content = JsonContent.Create(cmd)
        };
        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetUser_ValidToken_ReturnsOK()
    {
        var user = await _manager.FindByIdAsync("1");
        var userDto = new UserDto
        {
            Id = user!.Id,
            UserName = user.UserName!,
            Email = user.Email!
        };
        var jwtToken = _tokenManager.GenerateAccessToken(userDto!);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

        var response = await _client.GetAsync("api/user/get");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("invalidToken")]
    public async Task GetUser_InvalidToken_ReturnsUnauthorized(string? token)
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.GetAsync("api/user/get");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

}
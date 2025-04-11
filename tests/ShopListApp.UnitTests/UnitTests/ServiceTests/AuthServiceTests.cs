using Moq;
using ShopListApp.Application.Services;
using ShopListApp.Core.Commands.Auth;
using ShopListApp.Core.Dtos;
using ShopListApp.Core.Exceptions;
using ShopListApp.Core.Interfaces.Identity;
using ShopListApp.Core.Interfaces.ILogger;
using ShopListApp.Core.Interfaces.IRepositories;
using ShopListApp.Core.Models;

namespace ShopListApp.UnitTests.UnitTests.ServiceTests;

public class AuthServiceTests
{
    private AuthService _authService;
    private Mock<IUserManager> _mockUserManager;
    private Mock<IDbLogger<UserDto>> _mockLogger;
    private Mock<ITokenManager> _mockTokenManager;
    private Mock<ITokenRepository> _mockTokenRepository;
    public AuthServiceTests()
    {
        _mockUserManager = new Mock<IUserManager>();
        _mockLogger = new Mock<IDbLogger<UserDto>>();
        _mockTokenManager = new Mock<ITokenManager>();
        _mockTokenRepository = new Mock<ITokenRepository>();
        _authService = new AuthService(_mockUserManager.Object, _mockLogger.Object, _mockTokenManager.Object, _mockTokenRepository.Object);
    }

    [Fact]
    public async Task RegisterUser_ValidUser_ReturnsTokens()
    {
        RegisterUserCommand cmd = new RegisterUserCommand
        {
            UserName = "test",
            Email = "test@gmail.com",
            Password = "Password123@"
        };
        _mockUserManager.Setup(x => x.FindByEmailAsync(cmd.Email)).ReturnsAsync((UserDto?)null);
        _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<UserDto>(), cmd.Password)).ReturnsAsync(true);
        _mockTokenManager.Setup(x => x.GenerateAccessToken(It.IsAny<UserDto>())).Returns("token");
        _mockTokenManager.Setup(x => x.GenerateRefreshToken()).Returns("refreshToken");
        _mockTokenManager.Setup(x => x.GetHashRefreshToken("refreshToken")).Returns("hashedRefreshToken");
        _mockTokenRepository.Setup(x => x.AddToken(It.IsAny<Token>())).ReturnsAsync(true);

        var response = await _authService.RegisterUser(cmd);

        Assert.Equal("token", response.IdentityToken);
        Assert.Equal("refreshToken", response.RefreshToken);
    }

    [Fact]
    public async Task RegisterUser_UserWithEmailExists_ThrowsUserWithEmailAlreadyExistsException()
    {
        RegisterUserCommand cmd = new RegisterUserCommand
        {
            UserName = "test",
            Email = "test@gmail.com",
            Password = "Password123@"
        };
        var userDto = new UserDto
        {
            Id = "1",
            UserName = "test",
            Email = "test@gmail.com"
        };
        _mockUserManager.Setup(x => x.FindByEmailAsync(cmd.Email)).ReturnsAsync(userDto);

        async Task task() => await _authService.RegisterUser(cmd);

        await Assert.ThrowsAsync<UserWithEmailAlreadyExistsException>(task);
    }

    [Fact]
    public async Task RegisterUser_UserWithUserNameExists_ThrowsUserWithUserNameAlreadyExistsException()
    {
        RegisterUserCommand cmd = new RegisterUserCommand
        {
            UserName = "test",
            Email = "test@gmail.com",
            Password = "Password123@"
        };
        var userDto = new UserDto
        {
            Id = "1",
            UserName = "test",
            Email = "test@gmail.com"
        };
        _mockUserManager.Setup(x => x.FindByNameAsync(cmd.UserName)).ReturnsAsync(userDto);

        Func<Task> task = async () => await _authService.RegisterUser(cmd);

        await Assert.ThrowsAsync<UserWithUserNameAlreadyExistsException>(task);
    }

    [Fact]
    public async Task RegisterUser_NullArg_ThrowsArgumentNullException()
    {
        RegisterUserCommand cmd = null!;

        Func<Task> task = async () => await _authService.RegisterUser(cmd);

        await Assert.ThrowsAsync<ArgumentNullException>(task);
    }

    [Fact]
    public async Task RegisterUser_DatabaseError_ThrowsDatabaseErrorException()
    {
        RegisterUserCommand cmd = new RegisterUserCommand
        {
            UserName = "test",
            Email = "test@gmail.com",
            Password = "Password123@"
        };
        _mockUserManager.Setup(x => x.FindByEmailAsync(cmd.Email)).ReturnsAsync((UserDto?)null);
        _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<UserDto>(), cmd.Password)).ThrowsAsync(new Exception());

        Func<Task> task = async () => await _authService.RegisterUser(cmd);

        await Assert.ThrowsAsync<DatabaseErrorException>(task);
    }

    [Fact]
    public async Task LoginUser_ValidUsernameAndPassword_ReturnsTokens()
    {
        LoginUserCommand cmd = new LoginUserCommand
        {
            UserIdentifier = "test@gmail.com",
            Password = "Password123@"
        };
        var userDto = new UserDto
        {
            Id = "1",
            UserName = "test",
            Email = "test@gmail.com"
        };
        _mockUserManager.Setup(x => x.FindByEmailAsync(cmd.UserIdentifier)).ReturnsAsync(userDto);
        _mockUserManager.Setup(x => x.CheckPasswordAsync(It.IsAny<UserDto>(), cmd.Password)).ReturnsAsync(true);
        _mockTokenManager.Setup(x => x.GenerateAccessToken(It.IsAny<UserDto>())).Returns("token");
        _mockTokenManager.Setup(x => x.GenerateRefreshToken()).Returns("refreshToken");
        _mockTokenManager.Setup(x => x.GetHashRefreshToken("refreshToken")).Returns("hashedRefreshToken");
        _mockTokenRepository.Setup(x => x.AddToken(It.IsAny<Token>())).ReturnsAsync(true);

        var response = await _authService.LoginUser(cmd);

        Assert.Equal("token", response.IdentityToken);
        Assert.Equal("refreshToken", response.RefreshToken);
    }

    [Fact]
    public async Task LoginUser_ValidEmailAndPassword_ReturnsTokens()
    {
        LoginUserCommand cmd = new LoginUserCommand
        {
            UserIdentifier = "test@gmail.com",
            Password = "Password123@"
        };
        var userDto = new UserDto
        {
            Id = "1",
            UserName = "test",
            Email = "test@gmail.com"
        };
        _mockUserManager.Setup(x => x.FindByEmailAsync(cmd.UserIdentifier)).ReturnsAsync(userDto);
        _mockUserManager.Setup(x => x.CheckPasswordAsync(It.IsAny<UserDto>(), cmd.Password)).ReturnsAsync(true);
        _mockTokenManager.Setup(x => x.GenerateAccessToken(It.IsAny<UserDto>())).Returns("token");
        _mockTokenManager.Setup(x => x.GenerateRefreshToken()).Returns("refreshToken");
        _mockTokenManager.Setup(x => x.GetHashRefreshToken("refreshToken")).Returns("hashedRefreshToken");
        _mockTokenRepository.Setup(x => x.AddToken(It.IsAny<Token>())).ReturnsAsync(true);

        var response = await _authService.LoginUser(cmd);

        Assert.Equal("token", response.IdentityToken);
        Assert.Equal("refreshToken", response.RefreshToken);
    }

    [Fact]
    public async Task LoginUser_InvalidIdentifier_ThrowsUnauthorizedAccessException()
    {
        LoginUserCommand cmd = new LoginUserCommand
        {
            UserIdentifier = "test",
            Password = "Password123@"
        };

        _mockUserManager.Setup(x => x.FindByNameAsync(cmd.UserIdentifier)).ReturnsAsync((UserDto?)null);
        _mockUserManager.Setup(x => x.FindByEmailAsync(cmd.UserIdentifier)).ReturnsAsync((UserDto?)null);

        Func<Task> task = async () => await _authService.LoginUser(cmd);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(task);
    }

    [Fact]
    public async Task LoginUser_InvalidPassword_ThrowsUnauthorizedAccessException()
    {
        LoginUserCommand cmd = new LoginUserCommand
        {
            UserIdentifier = "test",
            Password = "Password123@"
        };
        var userDto = new UserDto
        {
            Id = "1",
            UserName = "test",
            Email = "test@gmail.com"
        };
        _mockUserManager.Setup(x => x.FindByNameAsync(cmd.UserIdentifier)).ReturnsAsync(userDto);
        _mockUserManager.Setup(x => x.CheckPasswordAsync(It.IsAny<UserDto>(), cmd.Password)).ReturnsAsync(false);

        Func<Task> task = async () => await _authService.LoginUser(cmd);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(task);
    }

    [Fact]
    public async Task LoginUser_NullArg_ThrowsArgumentNullException()
    {
        LoginUserCommand cmd = null!;

        Func<Task> task = async () => await _authService.LoginUser(cmd);

        await Assert.ThrowsAsync<ArgumentNullException>(task);
    }

    [Fact]
    public async Task LoginUser_DatabaseError_ThrowsDatabaseErrorException()
    {
        LoginUserCommand cmd = new LoginUserCommand
        {
            UserIdentifier = "test",
            Password = "Password123@"
        };
        _mockUserManager.Setup(x => x.FindByEmailAsync(cmd.UserIdentifier)).ReturnsAsync((UserDto?)null);
        _mockUserManager.Setup(x => x.FindByNameAsync(cmd.UserIdentifier)).ThrowsAsync(new Exception());

        Func<Task> task = async () => await _authService.LoginUser(cmd);

        await Assert.ThrowsAsync<DatabaseErrorException>(task);
    }

    [Fact]
    public async Task RefreshAccessToken_ValidToken_ReturnsToken()
    {
        var cmd = new RefreshTokenCommand
        {
            RefreshToken = "refreshToken"
        };
        var userDto = new UserDto
        {
            Id = "1",
            UserName = "test",
            Email = "test@gmail.com"
        };
        _mockTokenManager.Setup(x => x.GetHashRefreshToken(cmd.RefreshToken)).Returns("hash");
        _mockTokenRepository.Setup(x => x.GetToken("hash")).ReturnsAsync(new Token { UserId = userDto.Id, RefreshTokenHash = "hash" });
        _mockUserManager.Setup(x => x.FindByIdAsync("1")).ReturnsAsync(userDto);
        _mockTokenManager.Setup(x => x.GenerateAccessToken(It.IsAny<UserDto>())).Returns("token");

        string jwtToken = await _authService.RefreshAccessToken(cmd);

        Assert.Equal("token", jwtToken);
    }

    [Fact]
    public async Task RefreshAccessToken_InvalidToken_ThrowsUnauthorizedAccessException()
    {
        var cmd = new RefreshTokenCommand
        {
            RefreshToken = "refreshToken"
        };
        _mockTokenManager.Setup(x => x.GetHashRefreshToken(cmd.RefreshToken)).Returns("hash");
        _mockTokenRepository.Setup(x => x.GetToken("hash")).ReturnsAsync((Token?)null);

        Func<Task> task = async () => await _authService.RefreshAccessToken(cmd);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(task);
    }

    [Fact]
    public async Task RefreshAccessToken_NullArg_ThrowsArgumentNullException()
    {
        RefreshTokenCommand cmd = null!;

        Func<Task> task = async () => await _authService.RefreshAccessToken(cmd);

        await Assert.ThrowsAsync<ArgumentNullException>(task);
    }

    [Fact]
    public async Task RefreshAccessToken_DatabaseError_ThrowsDatabaseErrorException()
    {
        var cmd = new RefreshTokenCommand
        {
            RefreshToken = "refreshToken"
        };
        _mockTokenManager.Setup(x => x.GetHashRefreshToken(cmd.RefreshToken)).Returns("hash");
        _mockTokenRepository.Setup(x => x.GetToken("hash")).ThrowsAsync(new Exception());

        Func<Task> task = async () => await _authService.RefreshAccessToken(cmd);

        await Assert.ThrowsAsync<DatabaseErrorException>(task);
    }
}

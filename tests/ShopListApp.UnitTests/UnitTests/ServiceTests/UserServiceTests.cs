using Microsoft.AspNetCore.Identity;
using Moq;
using ShopListApp.Application.Services;
using ShopListApp.Core.Commands.Auth;
using ShopListApp.Core.Commands.Delete;
using ShopListApp.Core.Commands.Update;
using ShopListApp.Core.Dtos;
using ShopListApp.Core.Exceptions;
using ShopListApp.Core.Interfaces.Identity;
using ShopListApp.Core.Interfaces.ILogger;
using ShopListApp.Infrastructure.Database.Identity.AppUser;

namespace ShopListApp.UnitTests.UnitTests.ServiceTests;

public class UserServiceTests
{
    private Mock<IUserManager> _mockManager;
    private Mock<IDbLogger<UserDto>> _mockLogger;
    private UserService _userService;
    public UserServiceTests()
    {
        var store = new Mock<IUserStore<User>>();
        _mockManager = new Mock<IUserManager>();
        _mockLogger = new Mock<IDbLogger<UserDto>>();
        _userService = new UserService(_mockLogger.Object, _mockManager.Object);
    }

    [Fact]
    public void CreateUser_ValidUser_CreatesUser()
    {
        RegisterUserCommand cmd = new RegisterUserCommand
        {
            UserName = "test",
            Email = "test@gmail.com",
            Password = "Password123@"
        };
        _mockManager.Setup(x => x.CreateAsync(It.IsAny<UserDto>(), cmd.Password)).ReturnsAsync(true);

        var task = _userService.CreateUser(cmd);

        Assert.True(task.IsCompletedSuccessfully);
    }

    [Fact]
    public async Task CreateUser_NullUser_ThrowsArgumentNullException()
    {
        RegisterUserCommand cmd = null!;

        Func<Task> task = async () => await _userService.CreateUser(cmd);

        await Assert.ThrowsAsync<ArgumentNullException>(task);
    }

    

    [Fact]
    public void DeleteUser_UserFound_DeletesUser()
    {
        string id = "1";
        DeleteUserCommand cmd = new DeleteUserCommand
        {
            Password = "Password123@"
        };
        var userDto = new UserDto
        {
            Id = "1",
            UserName = "test",
            Email = "test@gmail.com"
        };
        _mockManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(userDto);
        _mockManager.Setup(x => x.ChangePasswordAsync(It.IsAny<UserDto>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);
        _mockManager.Setup(x => x.DeleteAsync(It.IsAny<UserDto>())).ReturnsAsync(true);
        _mockManager.Setup(x => x.CheckPasswordAsync(It.IsAny<UserDto>(), cmd.Password)).ReturnsAsync(true);

        var task = _userService.DeleteUser(id, cmd);

        Assert.True(task.IsCompletedSuccessfully);
    }

    [Fact]
    public async Task DeleteUser_UserNotFound_ThrowsUnathorizedAccessException()
    {
        string id = "1";
        DeleteUserCommand cmd = new DeleteUserCommand
        {
            Password = "Password123@"
        };
        _mockManager.Setup(x => x.FindByIdAsync(id)).ReturnsAsync((UserDto?)null);

        Func<Task> task = async () => await _userService.DeleteUser(id, cmd);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(task);
    }

    [Fact]
    public async Task DeleteUser_InvalidPassword_ThrowsUnathorizedAccessException()
    {
        string id = "1";
        DeleteUserCommand cmd = new DeleteUserCommand
        {
            Password = "Password123@"
        };
        _mockManager.Setup(x => x.FindByIdAsync(id)).ReturnsAsync((UserDto?)null);
        _mockManager.Setup(x => x.CheckPasswordAsync(It.IsAny<UserDto>(), cmd.Password)).ReturnsAsync(false);

        Func<Task> task = async () => await _userService.DeleteUser(id, cmd);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(task);
    }

    [Fact]
    public async Task DeleteUser_NullCmd_ThrowsArgumentNullException()
    {
        string id = "1";
        DeleteUserCommand cmd = null!;

        Func<Task> task = async () => await _userService.DeleteUser(id, cmd);

        await Assert.ThrowsAsync<ArgumentNullException>(task);
    }

    [Fact]
    public async Task DeleteUser_NullId_ThrowsArgumentNullException()
    {
        string id = null!;
        DeleteUserCommand cmd = new DeleteUserCommand
        {
            Password = "Password123@"
        };

        Func<Task> task = async () => await _userService.DeleteUser(id, cmd);

        await Assert.ThrowsAsync<ArgumentNullException>(task);
    }

    [Fact]
    public void UpdateUser_UserFound_UpdatesUser()
    {
        string id = "1";
        UpdateUserCommand cmd = new UpdateUserCommand
        {
            UserName = "updated",
            CurrentPassword = "Password123@"
        };
        var userDto = new UserDto
        {
            Id = "1",
            UserName = "test",
            Email = "test@gmail.com"
        };
        _mockManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(userDto);
        _mockManager.Setup(x => x.UpdateAsync(It.IsAny<UserDto>())).ReturnsAsync(true);
        _mockManager.Setup(x => x.ChangePasswordAsync(It.IsAny<UserDto>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);

        var task = _userService.UpdateUser(id, cmd);

        Assert.True(task.IsCompletedSuccessfully);
    }

    [Fact]
    public async Task UpdateUser_NotFound_ThrowsUnathorizedAccessException()
    {
        string id = "1";
        UpdateUserCommand cmd = new UpdateUserCommand
        {
            UserName = "updated",
            CurrentPassword = "Password123@"
        };
        _mockManager.Setup(x => x.FindByIdAsync(id)).ReturnsAsync((UserDto?)null);

        Func<Task> task = async () => await _userService.UpdateUser(id, cmd);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(task);
    }

    [Fact]
    public async Task UpdateUser_InvalidPassword_ThrowsUnathorizedAccessException()
    {
        string id = "1";
        UpdateUserCommand cmd = new UpdateUserCommand
        {
            UserName = "updated",
            CurrentPassword = "Password123@"
        };
        _mockManager.Setup(x => x.ChangePasswordAsync(It.IsAny<UserDto>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(false);

        Func<Task> task = async () => await _userService.UpdateUser(id, cmd);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(task);
    }

    public static IEnumerable<object[]> GetNullArgsForUpdateUser()
    {
        yield return new object[] { null!, null! };
        yield return new object[] { "123", null! };
        yield return new object[] { null!, new UpdateUserCommand { UserName = "test", CurrentPassword = "Password123@" } };
    }

    [Theory]
    [MemberData(nameof(GetNullArgsForUpdateUser))]
    public async Task UpdateUser_NullArgs_ThrowsArgumentNullException(string? id, UpdateUserCommand? cmd)
    {
        Func<Task> task = async () => await _userService.UpdateUser(id!, cmd!);

        await Assert.ThrowsAsync<ArgumentNullException>(task);
    }

    [Fact]
    public async Task GetUserById_UserFound_ReturnsUser()
    {
        string id = "1";
        var userDto = new UserDto
        {
            Id = "1",
            UserName = "test",
            Email = "test@gmail.com"
        };
        _mockManager.Setup(x => x.FindByIdAsync(id)).ReturnsAsync(userDto);

        var userView = await _userService.GetUserById(id);

        Assert.Equal("test", userView.UserName);
    }

    [Fact]
    public async Task GetUserById_UserNotFound_ThrowsUnathorizedAccessException()
    {
        string id = "1";
        _mockManager.Setup(x => x.FindByIdAsync(id)).ReturnsAsync((UserDto?)null);

        Func<Task> task = async () => await _userService.GetUserById(id);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(task);
    }

    

    [Fact]
    public async Task GetUserById_NullArg_ThrowsArgumentNullException()
    {
        string id = null!;

        Func<Task> task = async () => await _userService.GetUserById(id);

        await Assert.ThrowsAsync<ArgumentNullException>(task);
    }
}

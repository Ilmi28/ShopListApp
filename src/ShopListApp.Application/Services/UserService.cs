using ShopListApp.Core.Commands.Auth;
using ShopListApp.Core.Commands.Delete;
using ShopListApp.Core.Commands.Update;
using ShopListApp.Core.Dtos;
using ShopListApp.Core.Enums;
using ShopListApp.Core.Exceptions;
using ShopListApp.Core.Interfaces.Identity;
using ShopListApp.Core.Interfaces.ILogger;
using ShopListApp.Core.Interfaces.IServices;
using ShopListApp.Core.Responses;

namespace ShopListApp.Application.Services;

public class UserService(IDbLogger<UserDto> logger, IUserManager userManager) : IUserService
{
    public async Task CreateUser(RegisterUserCommand cmd)
    {
        _ = cmd ?? throw new ArgumentNullException(nameof(cmd));
        var user = new UserDto
        {
            Id = Guid.NewGuid().ToString(),
            UserName = cmd.UserName,
            Email = cmd.Email,
        };
        await userManager.CreateAsync(user, cmd.Password);
        await logger.Log(Operation.Create, user);
    }

    public async Task DeleteUser(string id, DeleteUserCommand cmd)
    {
        _ = id ?? throw new ArgumentNullException(nameof(id));
        _ = cmd ?? throw new ArgumentNullException(nameof(cmd));
        var user = await userManager.FindByIdAsync(id);
        if (user == null)
            throw new UnauthorizedAccessException();
        bool isPasswordCorrect = await userManager.CheckPasswordAsync(user, cmd.Password);
        if (!isPasswordCorrect)
            throw new UnauthorizedAccessException();
        var result = await userManager.DeleteAsync(user);
        if (!result)
            throw new UnauthorizedAccessException();
        await logger.Log(Operation.Delete, user);
    }

    public async Task UpdateUser(string id, UpdateUserCommand cmd)
    {
        _ = id ?? throw new ArgumentNullException(nameof(id));
        _ = cmd ?? throw new ArgumentNullException(nameof(cmd));
        var user = await userManager.FindByIdAsync(id) ?? throw new UnauthorizedAccessException();
        var existingUserWithUserName = await userManager.FindByNameAsync(cmd.UserName ?? string.Empty);
        if (existingUserWithUserName != null && cmd.UserName != user.UserName)
            throw new UserAlreadyExistsException("User with the given username already exists.");
        var existingUserWithEmail = await userManager.FindByEmailAsync(cmd.Email ?? string.Empty);
        if (existingUserWithEmail != null && cmd.Email != user.Email)  
            throw new UserAlreadyExistsException("User with the given email already exists.");
        user.UserName = cmd.UserName ?? user.UserName;
        user.Email = cmd.Email ?? user.Email;
        var result = await userManager.UpdateAsync(user);
        if (cmd.NewPassword != null)
        {
            var passwordResult = await userManager.ChangePasswordAsync(user,
                cmd.CurrentPassword,
                cmd.NewPassword ?? cmd.CurrentPassword);
            if (!result || !passwordResult)
                throw new UnauthorizedAccessException();
        }
        int propertiesCount = typeof(UpdateUserCommand).GetProperties().Length;
        int nullCount = GetLengthOfNullProperties(cmd);
        if (nullCount < propertiesCount - 1)
            await logger.Log(Operation.Update, user);
    }

    private int GetLengthOfNullProperties(UpdateUserCommand cmd)
    {
        int propertiesCount = typeof(UpdateUserCommand).GetProperties().Length;
        int nullCount = typeof(UpdateUserCommand)
                            .GetProperties()
                            .Count(p => p.GetValue(cmd) == null);
        return nullCount;
    }

    public async Task<UserResponse> GetUserById(string id)
    {
        _ = id ?? throw new ArgumentNullException(nameof(id));
        var user = await userManager.FindByIdAsync(id);
        if (user == null)
            throw new UnauthorizedAccessException();
        var view = new UserResponse
        {
            UserName = user.UserName!,
            Email = user.Email!
        };
        return view;
    }
}

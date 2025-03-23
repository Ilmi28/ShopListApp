using Microsoft.AspNetCore.Identity;
using ShopListApp.Commands;
using ShopListApp.Enums;
using ShopListApp.Exceptions;
using ShopListApp.Interfaces;
using ShopListApp.Models;
using ShopListApp.ViewModels;

namespace ShopListApp.Services
{
    public class UserService : IUserService
    {
        private IDbLogger<User> _logger;
        private UserManager<User> _userManager;
        public UserService(IDbLogger<User> logger, UserManager<User> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }

        public async Task CreateUser(CreateUserCommand cmd)
        {
            _ = cmd ?? throw new ArgumentNullException(nameof(cmd));
            var user = new User
            {
                UserName = cmd.UserName,
                Email = cmd.Email,
            };
            try
            {
                await _userManager.CreateAsync(user, cmd.Password);
                await _logger.Log(Operation.Create, user);
            }
            catch 
            {
                throw new DatabaseErrorException();
            }
        }

        public async Task DeleteUser(string id, DeleteUserCommand cmd)
        {
            _ = id ?? throw new ArgumentNullException(nameof(id));
            _ = cmd ?? throw new ArgumentNullException(nameof(cmd));
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                    throw new UnauthorizedAccessException();
                bool isPasswordCorrect = await _userManager.CheckPasswordAsync(user, cmd.Password);
                if (!isPasswordCorrect)
                    throw new UnauthorizedAccessException();
                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                    throw new UnauthorizedAccessException();
                await _logger.Log(Operation.Delete, new User { Id = id });
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

        public async Task UpdateUser(string id, UpdateUserCommand cmd)
        {
            _ = id ?? throw new ArgumentNullException(nameof(id));
            _ = cmd ?? throw new ArgumentNullException(nameof(cmd));
            try
            {
                var user = await _userManager.FindByIdAsync(id) ?? throw new UnauthorizedAccessException();
                user.UserName = cmd.UserName ?? user.UserName;
                user.Email = cmd.Email ?? user.Email;
                var result = await _userManager.UpdateAsync(user);
                var passwordResult = await _userManager.ChangePasswordAsync(user, 
                    cmd.CurrentPassword, 
                    cmd.NewPassword ?? cmd.CurrentPassword);
                if (!result.Succeeded || !passwordResult.Succeeded)
                    throw new UnauthorizedAccessException();
                int propertiesCount = typeof(UpdateUserCommand).GetProperties().Length;
                int nullCount = GetLengthOfNullProperties(cmd);
                if (nullCount < propertiesCount - 1)
                    await _logger.Log(Operation.Update, user);
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

        private int GetLengthOfNullProperties(UpdateUserCommand cmd)
        {
            int propertiesCount = typeof(UpdateUserCommand).GetProperties().Length;
            int nullCount = typeof(UpdateUserCommand)
                                .GetProperties()
                                .Count(p => p.GetValue(cmd) == null);
            return nullCount;
        }

        public async Task<UserView> GetUserById(string id)
        {
            _ = id ?? throw new ArgumentNullException(nameof(id));
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                    throw new UnauthorizedAccessException();
                var view = new UserView
                {
                    UserName = user.UserName!,
                    Email = user.Email!
                };
                return view;
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

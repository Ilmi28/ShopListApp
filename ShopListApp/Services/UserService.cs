using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ShopListApp.Commands;
using ShopListApp.Enums;
using ShopListApp.Exceptions;
using ShopListApp.Interfaces;
using ShopListApp.Models;
using ShopListApp.Repositories;

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

        public async Task DeleteUser(string id)
        {
            _ = id ?? throw new ArgumentNullException(nameof(id));
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
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

        public async Task UpdateUser(string id, UpdateUserCommand updatedUser)
        {
            _ = id ?? throw new ArgumentNullException(nameof(id));
            _ = updatedUser ?? throw new ArgumentNullException(nameof(updatedUser));
            try
            {
                var user = await _userManager.FindByIdAsync(id) ?? throw new UnauthorizedAccessException();
                user.UserName = updatedUser.UserName ?? user.UserName;
                user.Email = updatedUser.Email ?? user.Email;
                var result = await _userManager.UpdateAsync(user);
                var passwordResult = await _userManager.ChangePasswordAsync(user, 
                    updatedUser.CurrentPassword, 
                    updatedUser.NewPassword ?? updatedUser.CurrentPassword);
                if (!result.Succeeded || !passwordResult.Succeeded)
                    throw new UnauthorizedAccessException();
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

        public async Task<User?> GetUserById(string id)
        {
            _ = id ?? throw new ArgumentNullException(nameof(id));
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                    throw new UnauthorizedAccessException();
                return user;
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

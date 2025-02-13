using Microsoft.AspNetCore.Mvc.ModelBinding;
using ShopListApp.Commands.CreateCommands;
using ShopListApp.Commands.UpdateCommands;
using ShopListApp.Exceptions;
using ShopListApp.Interfaces;
using ShopListApp.Models;
using ShopListApp.Repositories;

namespace ShopListApp.Services
{
    public class UserService
    {
        private IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
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
                await _userRepository.CreateUser(user, cmd.Password);
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
                bool isValid = await _userRepository.DeleteUser(id);
                if (!isValid)
                    throw new UnauthorizedAccessException();
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
            var user = new User();
            user.UserName = updatedUser.UserName ?? user.UserName;
            user.Email = updatedUser.Email ?? user.Email;
            try
            {
                bool isValid = await _userRepository.UpdateUser(id, user, updatedUser.CurrentPassword, 
                    updatedUser.NewPassword ?? updatedUser.CurrentPassword);
                if (!isValid)
                    throw new UnauthorizedAccessException();
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
                var user = await _userRepository.GetUserById(id);
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

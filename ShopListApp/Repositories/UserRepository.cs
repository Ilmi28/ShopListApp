using Microsoft.AspNetCore.Identity;
using ShopListApp.Database;
using ShopListApp.Interfaces;
using ShopListApp.Models;

namespace ShopListApp.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<User> _userManager;
        public UserRepository(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<User?> GetUserById(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public async Task CreateUser(User user, string password)
        {
            await _userManager.CreateAsync(user, password);
        }

        public async Task<bool> UpdateUser(string id, User newUser, string currentPassword, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) 
                return false;
            user.UserName = newUser.UserName;
            user.Email = newUser.Email;
            await _userManager.UpdateAsync(newUser);
            var result = await _userManager.ChangePasswordAsync(newUser, currentPassword, newPassword);
            if (!result.Succeeded)
                return false;
            return true;
        }

        public async Task<bool> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) 
                return false;
            await _userManager.DeleteAsync(user);
            return true;
        }


    }
}

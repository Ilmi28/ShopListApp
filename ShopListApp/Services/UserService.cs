using Microsoft.AspNetCore.Mvc.ModelBinding;
using ShopListApp.Models;
using ShopListApp.Repositories;

namespace ShopListApp.Services
{
    public class UserService
    {
        private UserRepository _userRepository;
        public UserService(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task CreateUser(User user)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteUser(int id)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateUser(int id, User updatedUser)
        {
            throw new NotImplementedException();
        }

        public async Task<User?> GetUserById(int id)
        {
            throw new NotImplementedException();
        }
    }
}

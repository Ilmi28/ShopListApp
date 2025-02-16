using ShopListApp.Commands;
using ShopListApp.Models;

namespace ShopListApp.Interfaces
{
    public interface IUserService
    {
        Task CreateUser(CreateUserCommand cmd);
        Task DeleteUser(string id);
        Task UpdateUser(string id, UpdateUserCommand updatedUser);
        Task<User?> GetUserById(string id);
    }
}

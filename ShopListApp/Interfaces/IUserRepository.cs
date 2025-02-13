using ShopListApp.Models;

namespace ShopListApp.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetUserById(string id);
        Task CreateUser(User user, string password);
        Task<bool> UpdateUser(string id, User newUser, string currentPassword, string newPassword);
        Task<bool> DeleteUser(string id);
    }
}

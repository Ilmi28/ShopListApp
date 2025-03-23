using ShopListApp.Commands;
using ShopListApp.ViewModels;

namespace ShopListApp.Interfaces
{
    public interface IUserService
    {
        Task CreateUser(CreateUserCommand cmd);
        Task DeleteUser(string id, DeleteUserCommand cmd);
        Task UpdateUser(string id, UpdateUserCommand updatedUser);
        Task<UserView> GetUserById(string id);
    }
}

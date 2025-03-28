using ShopListApp.Commands;
using ShopListApp.Core.Commands.Auth;
using ShopListApp.Core.Commands.Delete;
using ShopListApp.ViewModels;

namespace ShopListApp.Interfaces
{
    public interface IUserService
    {
        Task CreateUser(RegisterUserCommand cmd);
        Task DeleteUser(string id, DeleteUserCommand cmd);
        Task UpdateUser(string id, UpdateUserCommand updatedUser);
        Task<UserResponse> GetUserById(string id);
    }
}

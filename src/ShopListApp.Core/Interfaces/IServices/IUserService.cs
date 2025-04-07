using ShopListApp.Core.Commands.Auth;
using ShopListApp.Core.Commands.Delete;
using ShopListApp.Core.Commands.Update;
using ShopListApp.Core.Responses;

namespace ShopListApp.Core.Interfaces.IServices
{
    public interface IUserService
    {
        Task CreateUser(RegisterUserCommand cmd);
        Task DeleteUser(string id, DeleteUserCommand cmd);
        Task UpdateUser(string id, UpdateUserCommand updatedUser);
        Task<UserResponse> GetUserById(string id);
    }
}

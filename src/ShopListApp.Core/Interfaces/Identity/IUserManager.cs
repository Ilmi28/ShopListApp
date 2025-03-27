using Microsoft.AspNetCore.Identity;
using ShopListApp.Core.Dtos;
using ShopListApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopListApp.Core.Interfaces
{
    public interface IUserManager
    {
        Task<bool> CreateAsync(UserDto user, string password);
        Task<UserDto?> FindByIdAsync(string userId);    
        Task<UserDto?> FindByEmailAsync(string email);
        Task<UserDto?> FindByNameAsync(string userName);
        Task<bool> CheckPasswordAsync(UserDto user, string password);
        Task<bool> DeleteAsync(UserDto user);
        Task<bool> UpdateAsync(UserDto user);
        Task<bool> ChangePasswordAsync(UserDto user, string currentPassword, string newPassword);
    }
}

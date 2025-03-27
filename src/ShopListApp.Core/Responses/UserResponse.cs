using System.ComponentModel.DataAnnotations;

namespace ShopListApp.ViewModels
{
    public class UserResponse
    {
        public required string UserName { get; set; }
        public required string Email { get; set; }
    }
}

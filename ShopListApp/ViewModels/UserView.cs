using System.ComponentModel.DataAnnotations;

namespace ShopListApp.ViewModels
{
    public class UserView
    {
        public required string UserName { get; set; }
        public required string Email { get; set; }
    }
}

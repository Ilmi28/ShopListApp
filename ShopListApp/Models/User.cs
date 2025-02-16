using Microsoft.AspNetCore.Identity;

namespace ShopListApp.Models
{
    public class User : IdentityUser
    {
        public bool IsDeleted { get; set; } = false;
    }
}

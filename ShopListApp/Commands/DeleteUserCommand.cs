using System.ComponentModel.DataAnnotations;

namespace ShopListApp.Commands
{
    public class DeleteUserCommand
    {
        [Required]
        public required string Password { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace ShopListApp.Core.Commands.Delete
{
    public class DeleteUserCommand
    {
        [Required]
        public required string Password { get; set; }
    }
}

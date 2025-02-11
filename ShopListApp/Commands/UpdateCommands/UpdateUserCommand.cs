using System.ComponentModel.DataAnnotations;

namespace ShopListApp.Commands.UpdateCommands
{
    public class UpdateUserCommand
    {
        [MinLength(3)]
        [MaxLength(50)]
        public string? UserName { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        [RegularExpression("^(?=.*[A-Z])(?=.*[a-z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{8,}$")]
        public string? Password { get; set; } 
    }
}

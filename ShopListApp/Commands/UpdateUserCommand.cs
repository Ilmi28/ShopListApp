using System.ComponentModel.DataAnnotations;

namespace ShopListApp.Commands
{
    public class UpdateUserCommand
    {
        [MinLength(3)]
        [MaxLength(50)]
        public string? UserName { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        [RegularExpression("^(?=.*[A-Z])(?=.*[a-z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{8,}$")]
        public required string CurrentPassword { get; set; }
        [RegularExpression("^(?=.*[A-Z])(?=.*[a-z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{8,}$")]
        public string? NewPassword { get; set; }
    }
}

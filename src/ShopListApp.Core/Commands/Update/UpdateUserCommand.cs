using System.ComponentModel.DataAnnotations;

namespace ShopListApp.Core.Commands.Update
{
    public class UpdateUserCommand
    {
        [MinLength(3)]
        [MaxLength(50)]
        public string? UserName { get; set; } = null;
        [EmailAddress]
        public string? Email { get; set; } = null;
        [Required]
        public required string CurrentPassword { get; set; }
        [RegularExpression("^(?=.*[A-Z])(?=.*[a-z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{8,}$")]
        public string? NewPassword { get; set; } = null;
    }
}

using System.ComponentModel.DataAnnotations;

namespace ShopListApp.ViewModels
{
    public class UserView
    {
        [Required]
        [MinLength(3)]
        [MaxLength(50)]
        public required string UserName { get; set; }
        [Required]
        [EmailAddress]
        public required string Email { get; set; }
        [Required]
        [RegularExpression("^(?=.*[A-Z])(?=.*[a-z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{8,}$")]
        public required string Password { get; set; }
    }
}

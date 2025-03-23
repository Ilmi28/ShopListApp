using System.ComponentModel.DataAnnotations;

namespace ShopListApp.Commands
{
    public class CreateShopListCommand
    {
        [Required]
        [MinLength(3)]
        public required string Name { get; set; }
    }
}

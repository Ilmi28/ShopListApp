using System.ComponentModel.DataAnnotations;

namespace ShopListApp.Commands
{
    public class UpdateShopListCommand
    {
        [Required]
        [MinLength(3)]
        public required string Name { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace ShopListApp.Core.Commands.Update
{
    public class UpdateShopListCommand
    {
        [Required]
        [MinLength(3)]
        public required string Name { get; set; }
    }
}

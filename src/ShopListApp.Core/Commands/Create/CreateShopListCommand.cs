using System.ComponentModel.DataAnnotations;

namespace ShopListApp.Core.Commands.Create;

public class CreateShopListCommand
{
    [Required]
    [MinLength(3)]
    public required string Name { get; set; }
}

namespace ShopListApp.Core.Models;

public class Store
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public bool IsDeleted { get; set; } = false;
}

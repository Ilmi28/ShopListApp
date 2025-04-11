namespace ShopListApp.Core.Responses;

public class ShopListResponse
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string OwnerId { get; set; }
    public required List<ProductResponse> Products { get; set; }
}

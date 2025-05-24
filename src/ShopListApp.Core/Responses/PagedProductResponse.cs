namespace ShopListApp.Core.Responses;

public class PagedProductResponse
{
    public int TotalProducts { get; set; }
    public ICollection<ProductResponse> Products { get; set; } =  new List<ProductResponse>();
}
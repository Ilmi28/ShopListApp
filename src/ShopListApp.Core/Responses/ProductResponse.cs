namespace ShopListApp.ViewModels
{
    public class ProductResponse
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public decimal? Price { get; set; }
        public int StoreId { get; set; }  
        public required string StoreName { get; set; }
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? ImageUrl { get; set; }
        public int Quantity { get; set; }
    }
}

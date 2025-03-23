namespace ShopListApp.Commands
{
    public class ParseProductCommand
    {
        public required string Name { get; set; }
        public decimal? Price { get; set; }
        public string? ImageUrl { get; set; }
        public string? CategoryName { get; set; }
        public int StoreId { get; set; }
    }
}

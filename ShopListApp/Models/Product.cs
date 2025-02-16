namespace ShopListApp.Models
{
    public class Product
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public decimal Price { get; set; }
        public int StoreId { get; set; }
        public required Store Store { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}

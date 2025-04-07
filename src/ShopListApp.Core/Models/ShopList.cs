namespace ShopListApp.Core.Models
{
    public class ShopList
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string UserId { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}

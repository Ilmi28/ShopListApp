namespace ShopListApp.ViewModels
{
    public class ShopListView
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string OwnerId { get; set; }
        public required List<ProductView> Products { get; set; }
    }
}

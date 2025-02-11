namespace ShopListApp.ViewModels
{
    public class ShopListView
    {
        public required string Name { get; set; }
        public required List<ProductView> Products { get; set; }
    }
}

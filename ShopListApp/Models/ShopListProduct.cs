namespace ShopListApp.Models
{
    public class ShopListProduct
    {
        public int Id { get; set; }
        public int ShopListId { get; set; }
        public required ShopList ShopList { get; set; }
        public int ProductId { get; set; }
        public required Product Product { get; set; }
    }
}

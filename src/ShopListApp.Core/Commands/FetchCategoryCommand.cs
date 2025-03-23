namespace ShopListApp.Commands
{
    public class FetchCategoryCommand
    {
        public required string Name { get; set; }
        public int StoreId { get; set; }
    }
}

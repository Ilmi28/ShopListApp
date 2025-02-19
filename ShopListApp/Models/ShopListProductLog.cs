using ShopListApp.Enums;

namespace ShopListApp.Models
{
    public class ShopListProductLog
    {
        public int Id { get; set; }
        public int ShopListId { get; set; }
        public int ProductId { get; set; }
        public Operation Operation { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
    }
}

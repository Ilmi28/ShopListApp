using ShopListApp.Core.Enums;

namespace ShopListApp.Core.Models
{
    public class ShopListLog
    {
        public int Id { get; set; }
        public int ShopListId { get; set; }
        public Operation Operation { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
    }
}

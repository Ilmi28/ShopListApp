using System.ComponentModel.DataAnnotations.Schema;

namespace ShopListApp.Models
{
    public class ShopListProduct
    {
        public int Id { get; set; }
        [ForeignKey("ShopListId")]
        public required ShopList ShopList { get; set; }
        [ForeignKey("ProductId")]
        public required Product Product { get; set; }
        public int Quantity { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}

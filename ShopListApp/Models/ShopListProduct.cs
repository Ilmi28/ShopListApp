using System.ComponentModel.DataAnnotations.Schema;

namespace ShopListApp.Models
{
    public class ShopListProduct
    {
        public int Id { get; set; }
        [ForeignKey("ShopList")]
        public int ShopListId { get; set; }
        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}

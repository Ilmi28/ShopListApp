using System.ComponentModel.DataAnnotations.Schema;

namespace ShopListApp.Models
{
    public class Product
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public decimal? Price { get; set; }
        [ForeignKey("Store")]
        public int StoreId { get; set; }
        public string? ImageUrl { get; set; }
        [ForeignKey("Category")]
        public int? CategoryId { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}

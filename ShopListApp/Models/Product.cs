using System.ComponentModel.DataAnnotations.Schema;

namespace ShopListApp.Models
{
    public class Product
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public decimal? Price { get; set; }
        [ForeignKey("StoreId")]
        public required Store Store { get; set; }
        public string? ImageUrl { get; set; }
        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}

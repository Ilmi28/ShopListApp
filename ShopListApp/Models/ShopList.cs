using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopListApp.Models
{
    public class ShopList
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        [ForeignKey("User")]
        public required string UserId { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}

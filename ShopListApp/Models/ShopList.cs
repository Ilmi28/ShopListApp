using System.Collections.Generic;

namespace ShopListApp.Models
{
    public class ShopList
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public int UserId { get; set; }
        public required User User { get; set; }
    }
}

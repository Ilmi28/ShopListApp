using System.ComponentModel.DataAnnotations.Schema;

namespace ShopListApp.Models
{
    public class Token
    {
        public int Id { get; set; }
        public required string RefreshTokenHash { get; set; }
        [ForeignKey("User")]
        public required string UserId { get; set; }
        public DateTime ExpirationDate { get; set; }
        public bool IsRevoked { get; set; } = false;
    }
}

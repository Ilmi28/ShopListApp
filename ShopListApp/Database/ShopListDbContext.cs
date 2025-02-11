
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShopListApp.Models;

namespace ShopListApp.Database
{
    public class ShopListDbContext : IdentityDbContext<User>
    {
        public ShopListDbContext(DbContextOptions<ShopListDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<ShopList> ShopLists { get; set; }
        public DbSet<ShopListProduct> ShopListProducts { get; set; }
        public DbSet<Store> Stores { get; set; }
    }
}

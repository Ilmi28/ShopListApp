
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
        public DbSet<ProductLog> ProductLogs { get; set; }
        public DbSet<ShopListLog> ShopListLogs { get; set; }
        public DbSet<UserLog> UserLogs { get; set; }
        public DbSet<Token> Tokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ShopListProduct>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<ShopList>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<Product>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<Store>().HasQueryFilter(x => !x.IsDeleted);
            base.OnModelCreating(modelBuilder);

        }   
    }
}

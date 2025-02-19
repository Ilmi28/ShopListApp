
using Microsoft.AspNetCore.Identity;
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
        public DbSet<ShopListProductLog> ShopListProductLogs { get; set; }
        public DbSet<UserLog> UserLogs { get; set; }
        public DbSet<Token> Tokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ShopListProduct>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<ShopList>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<Product>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<Store>().HasQueryFilter(x => !x.IsDeleted);

            modelBuilder.Ignore<IdentityRole>();  
            modelBuilder.Ignore<IdentityUserRole<Guid>>(); 
            modelBuilder.Ignore<IdentityUserClaim<Guid>>(); 
            modelBuilder.Ignore<IdentityUserLogin<Guid>>(); 
            modelBuilder.Ignore<IdentityUserToken<Guid>>(); 
            modelBuilder.Ignore<IdentityRoleClaim<Guid>>();

            modelBuilder.Entity<ShopListProductLog>().Property(x => x.Operation).HasConversion<string>();
            modelBuilder.Entity<UserLog>().Property(x => x.Operation).HasConversion<string>();
           
        }   
    }
}

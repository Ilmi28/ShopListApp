using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShopListApp.Core.Models;
using ShopListApp.Infrastructure.Database.Identity.AppUser;

namespace ShopListApp.Infrastructure.Database.Context;

public class ShopListDbContext(DbContextOptions options) : IdentityDbContext<User>(options)
{
    public DbSet<Product> Products { get; set; }
    public DbSet<ShopList> ShopLists { get; set; }
    public DbSet<ShopListProduct> ShopListProducts { get; set; }
    public DbSet<Store> Stores { get; set; }
    public DbSet<ShopListLog> ShopListLogs { get; set; }
    public DbSet<UserLog> UserLogs { get; set; }
    public DbSet<Token> Tokens { get; set; }
    public DbSet<Category> Categories { get; set; }

    protected virtual void SeedData(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "warzywa" },
            new Category { Id = 2, Name = "owoce" },
            new Category { Id = 3, Name = "pieczywa" },
            new Category { Id = 4, Name = "nabial i jajka" },
            new Category { Id = 5, Name = "mieso" },
            new Category { Id = 6, Name = "dania gotowe" },
            new Category { Id = 7, Name = "napoje" },
            new Category { Id = 8, Name = "mrozone" },
            new Category { Id = 9, Name = "artykuly spozywcze" },
            new Category { Id = 10, Name = "drogeria" },
            new Category { Id = 11, Name = "dla domu" },
            new Category { Id = 12, Name = "dla dzieci" },
            new Category { Id = 13, Name = "dla zwierzat" });

        modelBuilder.Entity<Store>().HasData(
            new Store { Id = 1, Name = "Biedronka" });
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ShopListProduct>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<ShopList>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<Store>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<Category>().HasIndex(x => x.Name).IsUnique();
        modelBuilder.Entity<Product>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<User>().HasIndex(x => x.Email).IsUnique();
        modelBuilder.Entity<User>().HasIndex(x => x.UserName).IsUnique();

        modelBuilder.Ignore<IdentityRole>();  
        modelBuilder.Ignore<IdentityUserRole<Guid>>(); 
        modelBuilder.Ignore<IdentityUserClaim<Guid>>(); 
        modelBuilder.Ignore<IdentityUserLogin<Guid>>(); 
        modelBuilder.Ignore<IdentityUserToken<Guid>>(); 
        modelBuilder.Ignore<IdentityRoleClaim<Guid>>();

        modelBuilder.Entity<ShopListLog>().Property(x => x.Operation).HasConversion<string>();
        modelBuilder.Entity<UserLog>().Property(x => x.Operation).HasConversion<string>();
        SeedData(modelBuilder);
    }
}   

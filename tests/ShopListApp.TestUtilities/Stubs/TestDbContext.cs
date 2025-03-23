using Microsoft.EntityFrameworkCore;
using ShopListApp.Database;
using ShopListApp.Models;

namespace ShopListAppTests.Stubs
{
    public class TestDbContext : ShopListDbContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
        {
        }

        protected override void SeedData(ModelBuilder modelBuilder)
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
    }
}

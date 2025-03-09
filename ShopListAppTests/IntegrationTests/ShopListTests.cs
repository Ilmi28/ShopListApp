using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using ShopListApp.Database;
using ShopListApp.Models;
using ShopListAppTests.IntegrationTests.WebApplicationFactories;
using ShopListAppTests.Stubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopListAppTests.IntegrationTests
{
    public class ShopListTests : IClassFixture<ShopListWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly ShopListDbContext _context;
        private readonly UserManager<User> _userManager;

        public ShopListTests(ShopListWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
            var scope = factory.Services.CreateScope();
            _context = scope.ServiceProvider.GetRequiredService<ShopListDbContext>();
            _userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
            SeedData();
        }

        private void SeedData()
        {
            var store = _context.Stores.First(x => x.Id == 1);
            var category1 = _context.Categories.First(x => x.Id == 1);
            var category2 = _context.Categories.First(x => x.Id == 2);
            var user = new User
            {
                Id = "1",
                UserName = "test",
                Email = "test@gmail.com"
            };
            _userManager.CreateAsync(user, "Password123@");
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Ziemniak myty jadalny 2 kg", Price = 7.99m, CategoryId = 1, StoreId = 1 },
                new Product { Id = 2, Name = "Papryka Czerwona luz", Price = 15.99m, CategoryId = 1, StoreId = 1 },
                new Product { Id = 3, Name = "Ogórki szklarniowe kg", Price = 13.99m, CategoryId = 1, StoreId = 1 }
            };
            _context.Products.AddRange(products);
            _context.SaveChanges();
            var shopList = new ShopList { Id = 1, Name = "Lista zakupów", UserId = user.Id };
            _context.ShopLists.Add(shopList);
            _context.SaveChanges();
            var shopListProducts = new List<ShopListProduct>
            {
                new ShopListProduct { Id = 1, ProductId = 1, ShopListId = 1 },
                new ShopListProduct { Id = 2, ProductId = 2, ShopListId = 1 },
                new ShopListProduct { Id = 3, ProductId = 3, ShopListId = 1 }
            };
            _context.ShopListProducts.AddRange(shopListProducts);
            _context.SaveChanges();
        }

        [Fact]
        public void Test()
        {

            Assert.Equal(3, _context.ShopListProducts.Count());
        }

    }
}

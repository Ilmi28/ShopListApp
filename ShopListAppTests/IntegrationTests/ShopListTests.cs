using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using ShopListApp.Commands;
using ShopListApp.Database;
using ShopListApp.Interfaces;
using ShopListApp.Models;
using ShopListAppTests.IntegrationTests.WebApplicationFactories;
using ShopListAppTests.Stubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace ShopListAppTests.IntegrationTests
{
    public class ShopListTests : IClassFixture<ShopListWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly ShopListDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly ITokenManager _tokenManager;

        public ShopListTests(ShopListWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
            var scope = factory.Services.CreateScope();
            _context = scope.ServiceProvider.GetRequiredService<ShopListDbContext>();
            _userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            _tokenManager = scope.ServiceProvider.GetRequiredService<ITokenManager>();
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
                new Product { Id = 1, Name = "Ziemniak myty jadalny 2 kg", Price = 7.99m, Category = category1, Store = store },
                new Product { Id = 2, Name = "Papryka Czerwona luz", Price = 15.99m, Category = category1, Store = store },
                new Product { Id = 3, Name = "Ogórki szklarniowe kg", Price = 13.99m, Category = category1, Store = store }
            };
            _context.Products.AddRange(products);
            _context.SaveChanges();
            var shopList = new ShopList { Id = 1, Name = "Lista zakupów", User = user };
            _context.ShopLists.Add(shopList);
            _context.SaveChanges();
            var shopListProducts = new List<ShopListProduct>
            {
                new ShopListProduct { Id = 1, Product = products[0], ShopList = shopList },
                new ShopListProduct { Id = 2, Product = products[1], ShopList = shopList },
                new ShopListProduct { Id = 3, Product = products[2], ShopList = shopList }
            };
            _context.ShopListProducts.AddRange(shopListProducts);
            _context.SaveChanges();
        }

        [Fact]
        public async Task CreateShopList_ValidModel_ReturnsOK()
        {
            var user = await _userManager.FindByIdAsync("1");
            var cmd = new CreateShopListCommand
            {
                Name = "Test shop list"
            };
            string jwtToken = _tokenManager.GenerateAccessToken(user!);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            var result = await _client.PostAsJsonAsync("api/shoplist/create", cmd);
            var dbShopList = _context.ShopLists.FirstOrDefault(x => x.Name == "Test shop list");

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal("Test shop list", dbShopList!.Name);
        }

        [Theory]
        [InlineData("invalidToken")]
        [InlineData(null!)]
        public async Task CreateShopList_InvalidToken_ReturnsUnathorized(string? token)
        {
            var cmd = new CreateShopListCommand
            {
                Name = "Test shop list"
            };

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var result = await _client.PostAsJsonAsync("api/shoplist/create", cmd);
            Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
        }

        [Theory]
        [InlineData("1")]
        [InlineData(null!)]
        public async Task CreateShopList_InvalidName_ReturnsBadRequest(string? name)
        {
            var user = await _userManager.FindByIdAsync("1");
            var cmd = new CreateShopListCommand
            {
                Name = name!
            };
            string jwtToken = _tokenManager.GenerateAccessToken(user!);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
            var result = await _client.PostAsJsonAsync("api/shoplist/create", cmd);
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task CreateShopList_NullRequest_ReturnsBadRequest()
        {
            var user = await _userManager.FindByIdAsync("1");
            string jwtToken = _tokenManager.GenerateAccessToken(user!);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            var result = await _client.PostAsJsonAsync("api/shoplist/create", (CreateShopListCommand?)null);

            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }

    }
}

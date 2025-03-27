using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ShopListApp.Infrastructure.Database.Context;
using ShopListApp.Models;
using ShopListAppTests.IntegrationTests.WebApplicationFactories;
using ShopListAppTests.Stubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ShopListAppTests.IntegrationTests
{
    public class ProductTests : IClassFixture<ProductWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly ShopListDbContext _context;

        public ProductTests(ProductWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
            var scope = factory.Services.CreateScope();
            _context = scope.ServiceProvider.GetRequiredService<ShopListDbContext>();
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
            SeedData();
        }

        private void SeedData()
        {
            var store = _context.Stores.First(x => x.Id == 1);
            var category1 = _context.Categories.First(x => x.Id == 1);
            var category2 = _context.Categories.First(x => x.Id == 2);
            _context.Products.AddRange(new List<Product>
            {
                new Product { Id = 1, Name = "Ziemniak myty jadalny 2 kg", Price = 7.99m, Category = category1, Store = store },
                new Product { Id = 2, Name = "Papryka Czerwona luz", Price = 15.99m, Category = category1, Store = store },
                new Product { Id = 3, Name = "Ogórki szklarniowe kg", Price = 13.99m, Category = category1, Store = store },
                new Product { Id = 4, Name = "Banany 1kg", Price = 4.99m, Category = category2, Store = store },
            });
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetAllProducts_ReturnsAllProducts()
        {
            var result = await _client.GetAsync("/api/product/get-all");

            var productCount = _context.Products.Count();
            var dbProducts = _context.Products.ToList();
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(4, productCount);
            Assert.Equal(1, dbProducts[0].Id);
            Assert.Equal(2, dbProducts[1].Id);
            Assert.Equal(3, dbProducts[2].Id);
            Assert.Equal(4, dbProducts[3].Id);
        }

        [Fact]
        public async Task GetProductsByCategory_CategoryFound_ReturnsAllProductsWithCategory()
        {
            var result = await _client.GetAsync("/api/product/get-by-category/1");
            var dbProducts = _context.Products.Where(x => x.Category!.Id == 1).ToList();
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(3, dbProducts.Count);
            Assert.Equal(1, dbProducts[0].Id);
            Assert.Equal(2, dbProducts[1].Id);
            Assert.Equal(3, dbProducts[2].Id);
        }

        [Fact]
        public async Task GetProductsByCategory_CategoryNotFound_ReturnsNotFound()
        {
            var result = await _client.GetAsync("/api/product/get-by-category/100");
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public async Task GetProductsByStore_StoreFound_ReturnsAllProductsWithStore()
        {
            var result = await _client.GetAsync("/api/product/get-by-store/1");
            var dbProducts = _context.Products.Where(x => x.Store.Id == 1).ToList();
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(4, dbProducts.Count);
            Assert.Equal(1, dbProducts[0].Id);
            Assert.Equal(2, dbProducts[1].Id);
            Assert.Equal(3, dbProducts[2].Id);
            Assert.Equal(4, dbProducts[3].Id);
        }

        [Fact]
        public async Task GetProductsByStore_StoreNotFound_ReturnsNotFound()
        {
            var result = await _client.GetAsync("/api/product/get-by-store/100");
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public async Task GetCategories_ReturnsAllCategories()
        {
            var result = await _client.GetAsync("/api/product/get-categories");
            var dbCategories = _context.Categories.ToList();
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(13, dbCategories.Count);
        }

        [Fact]
        public async Task GetProductById_ProductFound_ReturnsProductById()
        {
            var result = await _client.GetAsync("/api/product/get-product/1");
            var dbProduct = _context.Products.First(x => x.Id == 1);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(1, dbProduct.Id);
        }

        [Fact]
        public async Task GetProductById_ProductNotFound_ReturnsNotFound()
        {
            var result = await _client.GetAsync("/api/product/get-product/100");
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public async Task RefreshProducts_RefreshesProducts()
        {
            var result = await _client.PatchAsync("/api/product/refresh", null!);
            var dbProducts = _context.Products.ToList();

            foreach (var entity in _context.ChangeTracker.Entries().ToList())
                await entity.ReloadAsync();

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(3, dbProducts.Count);
            Assert.Equal(8.99m, dbProducts[0].Price);
            Assert.Equal(16.99m, dbProducts[1].Price);
            Assert.Equal(14.99m, dbProducts[2].Price);

        }
    }
}

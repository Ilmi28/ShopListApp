using Microsoft.AspNetCore.Mvc.Testing;
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
    public class StoreTests : IClassFixture<StoreWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly ShopListDbContext _context;
        public StoreTests (StoreWebApplicationFactory factory) 
        {
            _client = factory.CreateClient();
            var scope = factory.Services.CreateScope();
            _context = scope.ServiceProvider.GetRequiredService<ShopListDbContext>();
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
        }

        [Fact]
        public async Task GetAllStores_ReturnsAllStores()
        {
            var result = await _client.GetAsync("/api/store/get-all");
            var storeCount = _context.Stores.Count();
            var dbStores = _context.Stores.ToList();
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(1, storeCount);
            Assert.Equal(1, dbStores[0].Id);
        }

        [Fact]
        public async Task GetStoreById_StoreFound_ReturnsStore()
        {
            var result = await _client.GetAsync("/api/store/1");
            var dbStore = _context.Stores.First(x => x.Id == 1);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(1, dbStore.Id);
        }

        [Fact]
        public async Task GetStoreById_StoreNotFound_ReturnsNotFound()
        {
            var result = await _client.GetAsync("/api/store/get-by-id/2");
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }


    }
}

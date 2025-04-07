using Moq;
using ShopListApp.Application.Services;
using ShopListApp.Core.Exceptions;
using ShopListApp.Core.Interfaces.IRepositories;
using ShopListApp.Core.Models;

namespace ShopListApp.UnitTests.UnitTests.ServiceTests
{
    public class StoreServiceTests
    {
        private StoreService _storeService;
        private Mock<IStoreRepository> _mockStoreRepository;

        public StoreServiceTests()
        {
            _mockStoreRepository = new Mock<IStoreRepository>();
            _storeService = new StoreService(_mockStoreRepository.Object);
        }

        [Fact]
        public async Task GetStores_ReturnsAllStores()
        {
            var stores = new List<Store>
            {
                new Store { Id = 1, Name = "Store1" },
                new Store { Id = 2, Name = "Store2" },
                new Store { Id = 3, Name = "Store3" }
            };
            _mockStoreRepository.Setup(x => x.GetStores()).ReturnsAsync(stores);

            var result = await _storeService.GetStores();

            Assert.Equal(3, result.Count);
            Assert.Equal("Store1", result.ElementAt(0).Name);
            Assert.Equal("Store2", result.ElementAt(1).Name);
            Assert.Equal("Store3", result.ElementAt(2).Name);
        }

        [Fact]
        public async Task GetStores_NoStoresInDb_ReturnsEmptyList()
        {
            var stores = new List<Store>();
            _mockStoreRepository.Setup(x => x.GetStores()).ReturnsAsync(stores);
            var result = await _storeService.GetStores();
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetStores_DatabaseError_ThrowsDatabaseErrorException()
        {
            _mockStoreRepository.Setup(x => x.GetStores()).ThrowsAsync(new Exception());
            await Assert.ThrowsAsync<DatabaseErrorException>(() => _storeService.GetStores());
        }

        [Fact]
        public async Task GetStorebyId_ValidStoreId_ReturnsStores()
        {
            var store = new Store { Id = 1, Name = "Store1" };
            _mockStoreRepository.Setup(x => x.GetStoreById(1)).ReturnsAsync(store);
            var result = await _storeService.GetStoreById(1);
            Assert.Equal("Store1", result.Name);
        }

        [Fact]
        public async Task GetStorebyId_InvalidStoreId_ThrowsStoreNotFoundException()
        {
            _mockStoreRepository.Setup(x => x.GetStoreById(1)).ReturnsAsync((Store?)null);
            await Assert.ThrowsAsync<StoreNotFoundException>(() => _storeService.GetStoreById(1));
        }

        [Fact]
        public async Task GetStorebyId_DatabaseError_ThrowsDatabaseErrorException()
        {
            _mockStoreRepository.Setup(x => x.GetStoreById(1)).ThrowsAsync(new Exception());
            await Assert.ThrowsAsync<DatabaseErrorException>(() => _storeService.GetStoreById(1));
        }

    }
}

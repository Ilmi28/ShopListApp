﻿using Microsoft.AspNetCore.Identity;
using Moq;
using ShopListApp.Commands;
using ShopListApp.Exceptions;
using ShopListApp.Interfaces;
using ShopListApp.Interfaces.IRepositories;
using ShopListApp.Models;
using ShopListApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopListAppTests.UnitTests.ServiceTests
{
    public class ShopListServiceTests
    {
        private ShopListService _shopListService;
        private Mock<IShopListRepository> _mockShopListRepository;
        private Mock<IShopListProductRepository> _mockShopListProductRepository;
        private Mock<IProductRepository> _mockProductRepository;
        private Mock<IDbLogger<ShopList>> _mockLogger;
        private Mock<UserManager<User>> _mockUserManager;
        public ShopListServiceTests()
        {
            var store = new Mock<IUserStore<User>>();
            _mockShopListRepository = new Mock<IShopListRepository>();
            _mockShopListProductRepository = new Mock<IShopListProductRepository>();
            _mockProductRepository = new Mock<IProductRepository>();
            _mockLogger = new Mock<IDbLogger<ShopList>>();
            _mockUserManager = new Mock<UserManager<User>>(store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
            _shopListService = new ShopListService(_mockShopListRepository.Object,
                _mockShopListProductRepository.Object,
                _mockProductRepository.Object,
                _mockUserManager.Object,
                _mockLogger.Object);
        }

        [Fact]
        public void CreateShopList_ValidInput_CreatesShopList()
        {
            var cmd = new CreateShopListCommand { Name = "ShopList1" };
            var user = new User { Id = "1", UserName = "User1" };
            _mockUserManager.Setup(x => x.FindByIdAsync("1")).ReturnsAsync(user);
            _mockShopListRepository.Setup(x => x.AddShopList(It.IsAny<ShopList>())).Returns(Task.CompletedTask);

            var result = _shopListService.CreateShopList("1", cmd);

            Assert.True(result.IsCompletedSuccessfully);
        }

        [Fact]
        public async Task CreateShopList_InvalidUserId_ThrowsUserNotFoundException()
        {
            var cmd = new CreateShopListCommand { Name = "ShopList1" };
            _mockUserManager.Setup(x => x.FindByIdAsync("1")).ReturnsAsync((User?)null);
            _mockShopListRepository.Setup(x => x.AddShopList(It.IsAny<ShopList>())).Returns(Task.CompletedTask);

            Func<Task> task = () => _shopListService.CreateShopList("1", cmd);

            await Assert.ThrowsAsync<UserNotFoundException>(task);
        }

        [Fact]
        public async Task CreateShopList_DatabaseError_ThrowsDatabaseErrorException()
        {
            var cmd = new CreateShopListCommand { Name = "ShopList1" };
            var user = new User { Id = "1", UserName = "User1" };
            _mockUserManager.Setup(x => x.FindByIdAsync("1")).ReturnsAsync(user);
            _mockShopListRepository.Setup(x => x.AddShopList(It.IsAny<ShopList>())).ThrowsAsync(new Exception());

            Func<Task> task = () => _shopListService.CreateShopList("1", cmd);

            await Assert.ThrowsAsync<DatabaseErrorException>(task);
        }

        [Fact]
        public async Task CreateShopList_NullCmdArg_ThrowsArgumentNullException()
        {
            CreateShopListCommand? cmd = null;

            Func<Task> task = () => _shopListService.CreateShopList("1", cmd!);

            await Assert.ThrowsAsync<ArgumentNullException>(task);
        }

        [Fact]
        public async Task CreateShopList_NullStrArg_ThrowsArgumentNullException()
        {
            CreateShopListCommand cmd = new CreateShopListCommand { Name = "ShopList1" };

            Func<Task> task = () => _shopListService.CreateShopList(null!, cmd);

            await Assert.ThrowsAsync<ArgumentNullException>(task);
        }

        [Fact]
        public void AddProductToShopList_ValidArgs_AddsProduct()
        {
            var user = new User { Id = "1", UserName = "User1" };
            var store = new Store { Id = 1, Name = "Store1" };
            var category = new Category { Name = "Category1" };
            var shopList = new ShopList { Id = 1, Name = "ShopList1", User = user };
            var product = new Product { Id = 1, Name = "Product1", Category = category, Store = store };
            _mockShopListRepository.Setup(x => x.GetShopListById(1)).ReturnsAsync(shopList);
            _mockProductRepository.Setup(x => x.GetProductById(1)).ReturnsAsync(product);

            var result = _shopListService.AddProductToShopList(1, 1);

            Assert.True(result.IsCompletedSuccessfully);
        }

        [Fact]
        public async Task AddProductToShopList_InvalidShopListId_ThrowsShopListNotFoundException()
        {
            var user = new User { Id = "1", UserName = "User1" };
            var store = new Store { Id = 1, Name = "Store1" };
            var category = new Category { Name = "Category1" };
            var shopList = new ShopList { Id = 1, Name = "ShopList1", User = user };
            var product = new Product { Id = 1, Name = "Product1", Category = category, Store = store };
            _mockShopListRepository.Setup(x => x.GetShopListById(1)).ReturnsAsync((ShopList?)null);

            Func<Task> task = () => _shopListService.AddProductToShopList(1, 1);

            await Assert.ThrowsAsync<ShopListNotFoundException>(task);
        }

        [Fact]
        public async Task AddProductToShopList_InvalidProductId_ThrowsProductNotFoundException()
        {
            var user = new User { Id = "1", UserName = "User1" };
            var store = new Store { Id = 1, Name = "Store1" };
            var category = new Category { Name = "Category1" };
            var shopList = new ShopList { Id = 1, Name = "ShopList1", User = user };
            var product = new Product { Id = 1, Name = "Product1", Category = category, Store = store };
            _mockShopListRepository.Setup(x => x.GetShopListById(1)).ReturnsAsync((ShopList?)null);

            Func<Task> task = () => _shopListService.AddProductToShopList(1, 1);

            await Assert.ThrowsAsync<ShopListNotFoundException>(task);
        }

        [Fact]
        public async Task AddProductToShopList_DatabaseError_ThrowsDatabaseErrorException()
        {
            var user = new User { Id = "1", UserName = "User1" };
            var store = new Store { Id = 1, Name = "Store1" };
            var category = new Category { Name = "Category1" };
            var shopList = new ShopList { Id = 1, Name = "ShopList1", User = user };
            var product = new Product { Id = 1, Name = "Product1", Category = category, Store = store };
            _mockShopListRepository.Setup(x => x.GetShopListById(1)).ThrowsAsync(new Exception());

            Func<Task> task = () => _shopListService.AddProductToShopList(1, 1);

            await Assert.ThrowsAsync<DatabaseErrorException>(task);
        }

        [Fact]
        public void DeleteShopList_ValidShopListId_DeletesShopList()
        {
            var store = new Store { Id = 1, Name = "Store1" };
            var category = new Category { Name = "Category1" };
            var shopListProducts = new List<Product>
            {
                new Product { Id = 1, Name = "Product1", Store = store, Category = category },
                new Product { Id = 2, Name = "Product2", Store = store, Category = category },
                new Product { Id = 3, Name = "Product3", Store = store, Category = category }
            };
            var shopList = new ShopList { Id = 1, Name = "ShopList1", User = new User() };
            _mockShopListRepository.Setup(x => x.GetShopListById(1)).ReturnsAsync(shopList);
            _mockShopListProductRepository.Setup(x => x.GetProductsForShopList(1)).ReturnsAsync(shopListProducts);
            _mockShopListProductRepository.Setup(x => x.RemoveShopListProduct(1, 1)).ReturnsAsync(true);
            _mockShopListProductRepository.Setup(x => x.RemoveShopListProduct(1, 2)).ReturnsAsync(true);
            _mockShopListProductRepository.Setup(x => x.RemoveShopListProduct(1, 3)).ReturnsAsync(true);
            _mockShopListRepository.Setup(x => x.RemoveShopList(1)).Returns(Task.FromResult(true));

            var result = _shopListService.DeleteShopList(1);

            Assert.True(result.IsCompletedSuccessfully);
        }

        [Fact]
        public async Task DeleteShopList_InvalidShopListId_ThrowsShopListNotFoundException()
        {
            _mockShopListProductRepository.Setup(x => x.GetProductsForShopList(1)).ReturnsAsync(new List<Product>());
            _mockShopListRepository.Setup(x => x.RemoveShopList(1)).ReturnsAsync(false);

            Func<Task> task = () => _shopListService.DeleteShopList(1);

            await Assert.ThrowsAsync<ShopListNotFoundException>(task);
        }

        [Fact]
        public async Task DeleteShopList_DatabaseError_ThrowsDatabaseErrorException()
        {
            var shopList = new ShopList { Id = 1, Name = "ShopList1", User = new User() };
            _mockShopListRepository.Setup(x => x.GetShopListById(1)).ReturnsAsync(shopList);
            _mockShopListProductRepository.Setup(x => x.GetProductsForShopList(1)).ThrowsAsync(new Exception());
            Func<Task> task = () => _shopListService.DeleteShopList(1);
            await Assert.ThrowsAsync<DatabaseErrorException>(task);
        }

        [Fact]
        public async Task GetShopListById_ValidShopListId_ReturnsShopList()
        {
            var user = new User { Id = "1", UserName = "User1" };
            var shopList = new ShopList { Id = 1, Name = "ShopList1", User = user };
            var store = new Store { Id = 1, Name = "Store1" };
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Product1", Store = store },
                new Product { Id = 2, Name = "Product2", Store = store },
                new Product { Id = 3, Name = "Product3", Store = store }
            };
            var shopListProducts = new List<ShopListProduct>
            {
                new ShopListProduct { Product = products[0], ShopList = shopList },
                new ShopListProduct { Product = products[1], ShopList = shopList },
                new ShopListProduct { Product = products[2], ShopList = shopList }
            };
            _mockShopListProductRepository.Setup(x => x.GetProductsForShopList(1)).ReturnsAsync(products);
            _mockShopListRepository.Setup(x => x.GetShopListById(1)).ReturnsAsync(shopList);
            _mockProductRepository.Setup(x => x.GetProductById(1)).ReturnsAsync(products[0]);
            _mockProductRepository.Setup(x => x.GetProductById(2)).ReturnsAsync(products[1]);
            _mockProductRepository.Setup(x => x.GetProductById(3)).ReturnsAsync(products[2]);

            var returnedShopList = await _shopListService.GetShopListById(1);
            var returnedProducts = returnedShopList.Products;

            Assert.Equal("ShopList1", returnedShopList.Name);
            Assert.Equal(1, returnedProducts.ElementAt(0).Id);
            Assert.Equal(2, returnedProducts.ElementAt(1).Id);
            Assert.Equal(3, returnedProducts.ElementAt(2).Id);
        }

        [Fact]
        public async Task GetShopListProducts_InvalidShopListId_ThrowsShopListNotFoundException()
        {
            _mockShopListRepository.Setup(x => x.GetShopListById(1)).ReturnsAsync((ShopList?)null);

            Func<Task> task = () => _shopListService.GetShopListById(1);

            await Assert.ThrowsAsync<ShopListNotFoundException>(task);
        }

        [Fact]
        public async Task GetShopListProducts_DatabaseError_ThrowsDatabaseErrorException()
        {
            _mockShopListRepository.Setup(x => x.GetShopListById(1)).ThrowsAsync(new Exception());

            Func<Task> task = () => _shopListService.GetShopListById(1);

            await Assert.ThrowsAsync<DatabaseErrorException>(task);
        }

        [Fact]
        public void RemoveProductFromShopList_ValidArgs_RemovesProduct()
        {
            var shopList = new ShopList
            {
                Id = 1,
                Name = "ShopList1",
                User = new User()
            };
            _mockShopListProductRepository.Setup(x => x.RemoveShopListProduct(1, 1)).ReturnsAsync(true);
            _mockShopListRepository.Setup(x => x.GetShopListById(1)).ReturnsAsync(shopList);

            var result = _shopListService.RemoveProductFromShopList(1, 1);

            Assert.True(result.IsCompletedSuccessfully);
        }

        [Fact]
        public async Task RemoveProductFromShopList_InvalidShopListId_ThrowsShopListNotFoundException()
        {
            _mockShopListRepository.Setup(x => x.GetShopListById(1)).ReturnsAsync((ShopList?)null);
            _mockShopListProductRepository.Setup(x => x.RemoveShopListProduct(1, 1)).ReturnsAsync(false);

            Func<Task> task = () => _shopListService.RemoveProductFromShopList(1, 1);

            await Assert.ThrowsAsync<ShopListNotFoundException>(task);
        }

        [Fact]
        public async Task RemoveProductFromShopList_InvalidProductId_ThrowsShopListProductNotFoundException()
        {
            var shopList = new ShopList
            {
                Id = 1,
                Name = "ShopList1",
                User = new User()
            };
            _mockShopListRepository.Setup(x => x.GetShopListById(1)).ReturnsAsync(shopList);
            _mockShopListProductRepository.Setup(x => x.RemoveShopListProduct(1, 1)).ReturnsAsync(false);

            Func<Task> task = () => _shopListService.RemoveProductFromShopList(1, 1);

            await Assert.ThrowsAsync<ShopListProductNotFoundException>(task);
        }

        [Fact]
        public async Task RemoveProductFromShopList_DatabaseError_ThrowsDatabaseErrorException()
        {
            var shopList = new ShopList
            {
                Id = 1,
                Name = "ShopList1",
                User = new User()
            };
            _mockShopListRepository.Setup(x => x.GetShopListById(1)).ReturnsAsync(shopList);
            _mockShopListProductRepository.Setup(x => x.RemoveShopListProduct(1, 1)).ThrowsAsync(new Exception());

            Func<Task> task = () => _shopListService.RemoveProductFromShopList(1, 1);

            await Assert.ThrowsAsync<DatabaseErrorException>(task);
        }

        [Fact]
        public void UpdateShopList_ValidInput_UpdatesShopList()
        {
            var shopList = new ShopList { Id = 1, Name = "ShopList1", User = new User() };
            var cmd = new UpdateShopListCommand { Name = "ShopList2" };

            _mockShopListRepository.Setup(x => x.GetShopListById(1)).ReturnsAsync(shopList);
            _mockShopListRepository.Setup(x => x.UpdateShopList(1, It.IsAny<ShopList>())).ReturnsAsync(true);

            var result = _shopListService.UpdateShopList(1, cmd);

            Assert.True(result.IsCompletedSuccessfully);
        }

        [Fact]
        public async Task UpdateShopList_InvalidShopListId_ThrowsShopListNotFoundException()
        {
            var cmd = new UpdateShopListCommand { Name = "ShopList2" };
            _mockShopListRepository.Setup(x => x.GetShopListById(1)).ReturnsAsync((ShopList?)null);

            Func<Task> task = () => _shopListService.UpdateShopList(1, cmd);

            await Assert.ThrowsAsync<ShopListNotFoundException>(task);
        }

        [Fact]
        public async Task UpdateShopList_DatabaseError_ThrowsDatabaseErrorException()
        {
            var shopList = new ShopList { Id = 1, Name = "ShopList1", User = new User() };
            var updatedShopList = new ShopList { Id = 1, Name = "ShopList2", User = new User() };
            var cmd = new UpdateShopListCommand { Name = "ShopList2" };

            _mockShopListRepository.Setup(x => x.GetShopListById(1)).ReturnsAsync(shopList);
            _mockShopListRepository.Setup(x => x.UpdateShopList(1, updatedShopList)).ThrowsAsync(new Exception());

            Func<Task> task = () => _shopListService.UpdateShopList(1, cmd);

            await Assert.ThrowsAsync<DatabaseErrorException>(task);
        }

        [Fact]
        public async Task UpdateShopList_NullCmdArg_ThrowsArgumentNullException()
        {
            UpdateShopListCommand? cmd = null;
            Func<Task> task = () => _shopListService.UpdateShopList(1, cmd!);
            await Assert.ThrowsAsync<ArgumentNullException>(task);
        }
    }
}

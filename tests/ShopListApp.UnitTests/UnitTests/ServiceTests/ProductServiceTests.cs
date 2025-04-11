using Moq;
using ShopListApp.Application.Services;
using ShopListApp.Core.Exceptions;
using ShopListApp.Core.Interfaces.IRepositories;
using ShopListApp.Core.Interfaces.StoreObserver;
using ShopListApp.Core.Models;

namespace ShopListApp.UnitTests.UnitTests.ServiceTests;

public class ProductServiceTests
{
    private ProductService _productService;
    private Mock<IProductRepository> _mockProductRepository;
    private Mock<IStoreRepository> _mockStoreRepository;
    private Mock<ICategoryRepository> _mockCategoryRepository;
    private Mock<IStorePublisher> _mockStorePublisher;
    public ProductServiceTests()
    {
        _mockProductRepository = new Mock<IProductRepository>();
        _mockStoreRepository = new Mock<IStoreRepository>();
        _mockCategoryRepository = new Mock<ICategoryRepository>();
        _mockStorePublisher = new Mock<IStorePublisher>();
        _productService = new ProductService(_mockProductRepository.Object, _mockStoreRepository.Object, 
            _mockCategoryRepository.Object, _mockStorePublisher.Object);
    }

    [Fact]
    public async Task GetAllProducts_ReturnsAllProducts()
    {
        var store = new Store { Id = 1, Name = "Store1" };
        var category = new Category { Id = 1, Name = "Category1" };
        var products = new List<Product>
        {
            new Product { Id = 1, Name = "Product1", Price = 1.99m, Category = category, Store = store },
            new Product { Id = 2, Name = "Product2", Price = 2.99m, Category = category, Store = store },
            new Product { Id = 3, Name = "Product3", Price = 3.99m, Category = category, Store = store }
        };
        _mockProductRepository.Setup(x => x.GetAllProducts()).ReturnsAsync(products);
        _mockStoreRepository.Setup(x => x.GetStoreById(1)).ReturnsAsync(store);
        _mockCategoryRepository.Setup(x => x.GetCategoryById(1)).ReturnsAsync(category);

        var result = await _productService.GetAllProducts();

        Assert.Equal(3, result.Count);
        Assert.Equal("Product1", result.ElementAt(0).Name);
        Assert.Equal("Product2", result.ElementAt(1).Name);
        Assert.Equal("Product3", result.ElementAt(2).Name);
    }

    [Fact]
    public async Task GetAllProducts_NoProductsInDb_ReturnsEmptyList()
    {
        var products = new List<Product>();
        _mockProductRepository.Setup(x => x.GetAllProducts()).ReturnsAsync(products);
        var result = await _productService.GetAllProducts();
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllProducts_DatabaseError_ThrowsDatabaseErrorException()
    {
        _mockProductRepository.Setup(x => x.GetAllProducts()).ThrowsAsync(new Exception());
        await Assert.ThrowsAsync<DatabaseErrorException>(() => _productService.GetAllProducts());
    }

    [Fact]
    public async Task GetProductsByCategoryId_ValidCategoryId_ReturnsProducts()
    {
        var store = new Store { Id = 1, Name = "Store1" };
        var category = new Category { Id = 1, Name = "Category1" };
        var products = new List<Product>
        {
            new Product { Id = 1, Name = "Product1", Price = 1.99m, Category = category, Store = store },
            new Product { Id = 2, Name = "Product2", Price = 2.99m, Category = category, Store = store },
            new Product { Id = 3, Name = "Product3", Price = 3.99m, Category = category, Store = store }
        };
        _mockProductRepository.Setup(x => x.GetProductsByCategoryId(1)).ReturnsAsync(products);
        _mockStoreRepository.Setup(x => x.GetStoreById(1)).ReturnsAsync(store);
        _mockCategoryRepository.Setup(x => x.GetCategoryById(1)).ReturnsAsync(category);

        var result = await _productService.GetProductsByCategoryId(1);

        Assert.Equal(3, result.Count);
        Assert.Equal("Product1", result.ElementAt(0).Name);
        Assert.Equal("Product2", result.ElementAt(1).Name);
        Assert.Equal("Product3", result.ElementAt(2).Name);
    }

    [Fact]
    public async Task GetProductsByCategoryId_InvalidCategoryId_ThrowsCategoryNotFoundException()
    {
        _mockCategoryRepository.Setup(x => x.GetCategoryById(1)).ReturnsAsync((Category?)null);
        await Assert.ThrowsAsync<CategoryNotFoundException>(() => _productService.GetProductsByCategoryId(1));
    }

    [Fact]
    public async Task GetProductsByCategoryId_DatabaseError_ThrowsDatabaseErrorException()
    {
        _mockCategoryRepository.Setup(x => x.GetCategoryById(1)).ThrowsAsync(new Exception());
        await Assert.ThrowsAsync<DatabaseErrorException>(() => _productService.GetProductsByCategoryId(1));
    }

    [Fact]
    public async Task GetProductsByStoreId_ValidStoreId_ReturnsProducts()
    {
        var store = new Store { Id = 1, Name = "Store1" };
        var category = new Category { Id = 1, Name = "Category1" };
        var products = new List<Product>
        {
            new Product { Id = 1, Name = "Product1", Price = 1.99m, Category = category, Store = store },
            new Product { Id = 2, Name = "Product2", Price = 2.99m, Category = category, Store = store },
            new Product { Id = 3, Name = "Product3", Price = 3.99m, Category = category, Store = store }
        };
        _mockProductRepository.Setup(x => x.GetProductsByStoreId(1)).ReturnsAsync(products);
        _mockStoreRepository.Setup(x => x.GetStoreById(1)).ReturnsAsync(store);
        _mockCategoryRepository.Setup(x => x.GetCategoryById(1)).ReturnsAsync(category);

        var result = await _productService.GetProductsByStoreId(1);

        Assert.Equal(3, result.Count);
        Assert.Equal("Product1", result.ElementAt(0).Name);
        Assert.Equal("Product2", result.ElementAt(1).Name);
        Assert.Equal("Product3", result.ElementAt(2).Name);
    }

    [Fact]
    public async Task GetProductsByStoreId_InvalidStoreId_ThrowsStoreNotFoundException()
    {
        _mockStoreRepository.Setup(x => x.GetStoreById(1)).ReturnsAsync((Store?)null);
        await Assert.ThrowsAsync<StoreNotFoundException>(() => _productService.GetProductsByStoreId(1));
    }

    [Fact]
    public async Task GetProductsByStoreId_DatabaseError_ThrowsDatabaseErrorException()
    {
        _mockStoreRepository.Setup(x => x.GetStoreById(1)).ThrowsAsync(new Exception());
        await Assert.ThrowsAsync<DatabaseErrorException>(() => _productService.GetProductsByStoreId(1));
    }
}

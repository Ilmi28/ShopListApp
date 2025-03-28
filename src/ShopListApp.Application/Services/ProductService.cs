using ShopListApp.Core.Dtos;
using ShopListApp.Core.Interfaces.StoreObserver;
using ShopListApp.Exceptions;
using ShopListApp.Interfaces;
using ShopListApp.Interfaces.IRepositories;
using ShopListApp.Interfaces.IServices;
using ShopListApp.Models;
using ShopListApp.ViewModels;

namespace ShopListApp.Services
{
    public class ProductService : IProductService
    {
        private readonly IStorePublisher _storePublisher;
        private readonly IProductRepository _productRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly ICategoryRepository _categoryRepository;
        public ProductService(IProductRepository productRepository, IStoreRepository storeRepository, 
            ICategoryRepository categoryRepository, IStorePublisher storePublisher)
        {
            _storePublisher = storePublisher;
            _productRepository = productRepository;
            _storeRepository = storeRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<ICollection<ProductResponse>> GetAllProducts()
        {
            try
            {
                var products = await _productRepository.GetAllProducts();
                var productViews = GetProductViewsList(products);
                return productViews;
            }
            catch { throw new DatabaseErrorException(); }
        }

        public async Task<ICollection<ProductResponse>> GetProductsByCategoryId(int categoryId)
        {
            try
            {
                _ = await _categoryRepository.GetCategoryById(categoryId)
                    ?? throw new CategoryNotFoundException();
                var products = await _productRepository.GetProductsByCategoryId(categoryId);
                var productViews = GetProductViewsList(products);
                return productViews;
            }
            catch (CategoryNotFoundException) { throw; }
            catch { throw new DatabaseErrorException(); }
        }

        public async Task<ICollection<ProductResponse>> GetProductsByStoreId(int storeId)
        {
            try
            {
                _ = await _storeRepository.GetStoreById(storeId)
                    ?? throw new StoreNotFoundException();
                var products = await _productRepository.GetProductsByStoreId(storeId);
                var productViews = GetProductViewsList(products);
                return productViews;
            }
            catch (StoreNotFoundException) { throw; }
            catch { throw new DatabaseErrorException(); }
        }

        public async Task RefreshProducts()
        {
            try
            {
                _storePublisher.AddSubscribers();
                await _storePublisher.Notify();
            }
            catch { throw new FetchingErrorException(); }
        }

        public async Task<ICollection<CategoryResponse>> GetCategories()
        {
            try
            {
                var categories = await _categoryRepository.GetAllCategories();
                var categoryViews = new List<CategoryResponse>();
                foreach (var category in categories)
                {
                    var categoryView = new CategoryResponse
                    {
                        Id = category.Id,
                        Name = category.Name,
                    };
                    categoryViews.Add(categoryView);
                }
                return categoryViews;
            }
            catch { throw new DatabaseErrorException(); }
        }

        private ICollection<ProductResponse> GetProductViewsList(ICollection<Product> products)
        {
            var productViews = new List<ProductResponse>();
            foreach (var product in products)
            {
                var productView = new ProductResponse
                {
                    Id = product.Id,  
                    Name = product.Name,
                    Price = product.Price,
                    StoreId = product.Store.Id,
                    StoreName = product.Store.Name,
                    CategoryId = product.Category?.Id,
                    CategoryName = product.Category?.Name,
                    ImageUrl = product.ImageUrl,
                };
                productViews.Add(productView);
            }
            return productViews;
        }

        public async Task<ProductResponse?> GetProductById(int id)
        {
            try
            {
                var product = await _productRepository.GetProductById(id) ?? throw new ProductNotFoundException();
                var productView = new ProductResponse
                {
                    Id = product.Id,
                    Name = product.Name,
                    Price = product.Price,
                    StoreId = product.Store.Id,
                    CategoryId = product.Category?.Id,
                    CategoryName = product.Category?.Name,
                    StoreName = product.Store.Name,
                    ImageUrl = product.ImageUrl,
                };
                return productView;
            }
            catch (ProductNotFoundException) { throw; }
            catch { throw new DatabaseErrorException(); }
        }
    }
}

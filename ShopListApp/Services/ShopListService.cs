using Microsoft.AspNetCore.Identity;
using ShopListApp.Commands;
using ShopListApp.Enums;
using ShopListApp.Exceptions;
using ShopListApp.Interfaces;
using ShopListApp.Interfaces.IRepositories;
using ShopListApp.Interfaces.IServices;
using ShopListApp.Models;
using ShopListApp.ViewModels;

namespace ShopListApp.Services
{
    public class ShopListService : IShopListService
    {
        private readonly IShopListRepository _shopListRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IShopListProductRepository _shopListProductRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly IDbLogger<ShopListProduct> _logger;
        private readonly UserManager<User> _userManager;

        public ShopListService(IShopListRepository shopListRepository, IShopListProductRepository shopListProductRepository,
                                IProductRepository productRepository, UserManager<User> userManager, 
                                ICategoryRepository categoryRepository, IStoreRepository storeRepository,
                                IDbLogger<ShopListProduct> logger)
        {
            _shopListRepository = shopListRepository;
            _shopListProductRepository = shopListProductRepository;
            _productRepository = productRepository;
            _userManager = userManager;
            _categoryRepository = categoryRepository;
            _storeRepository = storeRepository;
            _logger = logger;
        }

        public async Task AddProductToShopList(int shopListId, int productId)
        {
            try
            {
                var shopList = await _shopListRepository.GetShopListById(shopListId) ?? throw new ShopListNotFoundException();
                var product = await _productRepository.GetProductById(productId) ?? throw new ProductNotFoundException();
                var shopListProduct = new ShopListProduct
                {
                    ShopListId = shopListId,
                    ProductId = productId,
                };
                await _shopListProductRepository.AddShopListProduct(shopListProduct);
                await _logger.Log(Operation.Create, shopListProduct);
            }
            catch (ShopListNotFoundException) { throw; }
            catch (ProductNotFoundException) { throw; }
            catch { throw new DatabaseErrorException(); }
        }

        public async Task CreateShopList(CreateShopListCommand cmd)
        {
            _ = cmd ?? throw new ArgumentNullException();
            try
            {
                var user = await _userManager.FindByIdAsync(cmd.UserId) ?? throw new UserNotFoundException();
                var shopList = new ShopList
                {
                    Name = cmd.Name,
                    UserId = cmd.UserId,
                };
                await _shopListRepository.AddShopList(shopList);
            }
            catch (UserNotFoundException) { throw; }
            catch { throw new DatabaseErrorException(); }
        }

        public async Task DeleteShopList(int shopListId)
        {
            bool result;
            try
            {
                var shopListProductsIds = await _shopListProductRepository.GetProductIdsForShopList(shopListId);
                foreach (int productId in shopListProductsIds)
                {
                    result = await _shopListProductRepository.RemoveShopListProduct(shopListId, productId);
                    if (!result) throw new ShopListProductNotFoundException();
                }
                result = await _shopListRepository.RemoveShopList(shopListId);
                if (!result) throw new ShopListNotFoundException();
            }
            catch (ShopListNotFoundException) { throw; }
            catch { throw new DatabaseErrorException(); }
        }

        public async Task<ICollection<ProductView>> GetShopListProducts(int shopListId)
        {
            try
            {
                var shopList = await _shopListRepository.GetShopListById(shopListId) ?? throw new ShopListNotFoundException();
                var shopListProductsIds = await _shopListProductRepository.GetProductIdsForShopList(shopListId);
                var productViews = new List<ProductView>();
                foreach (int productId in shopListProductsIds)
                {
                    var product = await _productRepository.GetProductById(productId) ?? throw new ProductNotFoundException();
                    var category = await _categoryRepository.GetCategoryById(product.CategoryId) ?? throw new CategoryNotFoundException();
                    var store = await _storeRepository.GetStoreById(product.StoreId) ?? throw new StoreNotFoundException();
                    productViews.Add(new ProductView
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Price = product.Price,
                        CategoryId = product.CategoryId,
                        CategoryName = category.Name,
                        StoreId = product.StoreId,
                        StoreName = store.Name,
                        ImageUrl = product.ImageUrl
                    });
                }
                return productViews;
            }
            catch (ProductNotFoundException) { throw; }
            catch (CategoryNotFoundException) { throw; }
            catch (ShopListNotFoundException) { throw; }
            catch { throw new DatabaseErrorException(); }
        }

        public async Task RemoveProductFromShopList(int shopListId, int productId)
        {
            try
            {
                bool result = await _shopListProductRepository.RemoveShopListProduct(shopListId, productId);
                if (!result) throw new ShopListProductNotFoundException();
            }
            catch (ShopListProductNotFoundException) { throw; }
            catch
            {
                throw new DatabaseErrorException();
            }
        }
    }
}

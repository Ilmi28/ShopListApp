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
        private readonly IShopListProductRepository _shopListProductRepository;
        private readonly IDbLogger<ShopList> _logger;
        private readonly UserManager<User> _userManager;

        public ShopListService(IShopListRepository shopListRepository, IShopListProductRepository shopListProductRepository,
                                IProductRepository productRepository, UserManager<User> userManager,
                                IDbLogger<ShopList> logger)
        {
            _shopListRepository = shopListRepository;
            _shopListProductRepository = shopListProductRepository;
            _productRepository = productRepository;
            _userManager = userManager;
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
                    ShopList = shopList,
                    Product = product
                };
                await _shopListProductRepository.AddShopListProduct(shopListProduct);
                await _logger.Log(Operation.Update, shopList);
            }
            catch (ShopListNotFoundException) { throw; }
            catch (ProductNotFoundException) { throw; }
            catch { throw new DatabaseErrorException(); }
        }

        public async Task CreateShopList(string userId, CreateShopListCommand cmd)
        {
            _ = userId ?? throw new ArgumentNullException();
            _ = cmd ?? throw new ArgumentNullException();
            try
            {
                var user = await _userManager.FindByIdAsync(userId) ?? throw new UserNotFoundException();
                var shopList = new ShopList
                {
                    Name = cmd.Name,
                    User = user
                };
                await _shopListRepository.AddShopList(shopList);
                await _logger.Log(Operation.Create, shopList);
            }
            catch (UserNotFoundException) { throw; }
            catch { throw new DatabaseErrorException(); }
        }

        public async Task DeleteShopList(int shopListId)
        {
            bool result;
            try
            {
                var shopList = await _shopListRepository.GetShopListById(shopListId) ?? throw new ShopListNotFoundException();
                var shopListProducts = await _shopListProductRepository.GetProductsForShopList(shopListId);
                foreach (var product in shopListProducts)
                {
                    result = await _shopListProductRepository.RemoveShopListProduct(shopListId, product.Id);
                    if (!result) throw new ShopListProductNotFoundException();
                }
                result = await _shopListRepository.RemoveShopList(shopListId);
                if (!result) throw new ShopListNotFoundException();
                await _logger.Log(Operation.Delete, shopList);

            }
            catch (ShopListNotFoundException) { throw; }
            catch { throw new DatabaseErrorException(); }
        }

        public async Task<ShopListView> GetShopListById(int shopListId)
        {
            try
            {
                var shopList = await _shopListRepository.GetShopListById(shopListId) ?? throw new ShopListNotFoundException();
                var shopListProducts = await _shopListProductRepository.GetProductsForShopList(shopListId);
                var productViews = new List<ProductView>();
                foreach (var product in shopListProducts)
                {
                    productViews.Add(new ProductView
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Price = product.Price,
                        CategoryId = product.Category?.Id,
                        CategoryName = product.Category?.Name,
                        StoreId = product.Store.Id,
                        StoreName = product.Store.Name,
                        ImageUrl = product.ImageUrl
                    });
                }
                var shopListView = new ShopListView
                {
                    Name = shopList.Name,
                    Products = productViews
                };
                return shopListView;
            }
            catch (ShopListNotFoundException) { throw; }
            catch { throw new DatabaseErrorException(); }
        }

        public async Task RemoveProductFromShopList(int shopListId, int productId)
        {
            try
            {
                var shopList = await _shopListRepository.GetShopListById(shopListId) ?? throw new ShopListNotFoundException();
                bool result = await _shopListProductRepository.RemoveShopListProduct(shopListId, productId);
                if (!result) throw new ShopListProductNotFoundException();
                await _logger.Log(Operation.Update, shopList);
            }
            catch (ShopListNotFoundException) { throw; }
            catch (ShopListProductNotFoundException) { throw; }
            catch
            {
                throw new DatabaseErrorException();
            }
        }

        public async Task UpdateShopList(int shopListId, UpdateShopListCommand cmd)
        {
            _ = cmd ?? throw new ArgumentNullException();
            try
            {
                var shopList = await _shopListRepository.GetShopListById(shopListId)
                    ?? throw new ShopListNotFoundException();
                var updatedShopList = new ShopList
                {
                    Name = cmd.Name,
                    User = shopList.User
                };
                if (shopList == null) throw new ShopListNotFoundException();
                var result = await _shopListRepository.UpdateShopList(shopListId, updatedShopList);
                if (!result) throw new DatabaseErrorException();
            }
            catch (ShopListNotFoundException) { throw; }
            catch { throw new DatabaseErrorException(); }
        }
    }
}

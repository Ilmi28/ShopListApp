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

        public async Task AddProductToShopList(int shopListId, int productId, int quantity = 1)
        {
            try
            {
                var shopList = await _shopListRepository.GetShopListById(shopListId) ?? throw new ShopListNotFoundException();
                var product = await _productRepository.GetProductById(productId) ?? throw new ProductNotFoundException();
                var dbShopListProduct = await _shopListProductRepository.GetShopListProduct(shopListId, productId);
                if (dbShopListProduct != null)
                {
                    int newQuantity = dbShopListProduct.Quantity + quantity;
                    await _shopListProductRepository.UpdateShopListProductQuantity(dbShopListProduct.Id, newQuantity);
                }
                else
                {
                    var newShopListProduct = new ShopListProduct
                    {
                        ShopList = shopList,
                        Product = product,
                        Quantity = quantity
                    };
                    await _shopListProductRepository.AddShopListProduct(newShopListProduct);
                }
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
            catch (UserNotFoundException) { throw new UnauthorizedAccessException(); }
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
                    var shopListProductForQuantity = await _shopListProductRepository.GetShopListProduct(shopListId, product.Id)
                        ?? throw new DatabaseErrorException();
                    productViews.Add(new ProductView
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Price = product.Price,
                        CategoryId = product.Category?.Id,
                        CategoryName = product.Category?.Name,
                        StoreId = product.Store.Id,
                        StoreName = product.Store.Name,
                        ImageUrl = product.ImageUrl,
                        Quantity = shopListProductForQuantity.Quantity
                    });
                }
                var shopListView = new ShopListView
                {
                    Id = shopListId,
                    Name = shopList.Name,
                    OwnerId = shopList.User.Id,
                    Products = productViews
                };
                return shopListView;
            }
            catch (ShopListNotFoundException) { throw; }
            catch { throw new DatabaseErrorException(); }
        }

        public async Task RemoveProductFromShopList(int shopListId, int productId, int quantity = Int32.MaxValue)
        {
            try
            {
                var shopList = await _shopListRepository.GetShopListById(shopListId) ?? throw new ShopListNotFoundException();
                var shopListProduct = await _shopListProductRepository.GetShopListProduct(shopListId, productId) 
                    ?? throw new ShopListProductNotFoundException();
                bool result;
                if (quantity >= shopListProduct.Quantity)
                    result = await _shopListProductRepository.RemoveShopListProduct(shopListId, productId);
                else
                {
                    int newQuantity = shopListProduct.Quantity - quantity;
                    result = await _shopListProductRepository.UpdateShopListProductQuantity(shopListProduct.Id, newQuantity);
                }
                if (!result) throw new DatabaseErrorException();
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

        public async Task<ICollection<ShopListView>> GetShopListsForUser(string userId)
        {
            _ = userId ?? throw new ArgumentNullException();
            try
            {
                var user = await _userManager.FindByIdAsync(userId) ?? throw new UserNotFoundException();
                var shopLists = await _shopListRepository.GetShopListsByUser(userId);
                var shopListViews = new List<ShopListView>();
                foreach (var shopList in shopLists)
                {
                    var productViews = await GetProductsViewsForShopList(shopList);
                    var shopListView = new ShopListView
                    {
                        Id = shopList.Id,
                        Name = shopList.Name,
                        OwnerId = shopList.User.Id,
                        Products = productViews
                    };
                    shopListViews.Add(shopListView);
                }
                return shopListViews;
            }
            catch (UserNotFoundException) { throw new UnauthorizedAccessException(); }
            catch { throw new DatabaseErrorException(); }
        }

        private async Task<List<ProductView>> GetProductsViewsForShopList(ShopList shopList)
        {
            var shopListProducts = await _shopListProductRepository.GetProductsForShopList(shopList.Id);
            var productViews = new List<ProductView>();
            foreach (var product in shopListProducts)
            {
                var shopListProductForQuantity = await _shopListProductRepository.GetShopListProduct(shopList.Id, product.Id)
                    ?? throw new DatabaseErrorException();
                productViews.Add(new ProductView
                {
                    Id = product.Id,
                    Name = product.Name,
                    Price = product.Price,
                    CategoryId = product.Category?.Id,
                    CategoryName = product.Category?.Name,
                    StoreId = product.Store.Id,
                    StoreName = product.Store.Name,
                    ImageUrl = product.ImageUrl,
                    Quantity = shopListProductForQuantity.Quantity
                });
            }
            return productViews;
        }
    }
}

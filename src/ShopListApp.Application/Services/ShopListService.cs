using ShopListApp.Core.Commands.Create;
using ShopListApp.Core.Commands.Update;
using ShopListApp.Core.Enums;
using ShopListApp.Core.Exceptions;
using ShopListApp.Core.Interfaces.Identity;
using ShopListApp.Core.Interfaces.ILogger;
using ShopListApp.Core.Interfaces.IRepositories;
using ShopListApp.Core.Interfaces.IServices;
using ShopListApp.Core.Models;
using ShopListApp.Core.Responses;

namespace ShopListApp.Application.Services;

public class ShopListService(IShopListRepository shopListRepository, IShopListProductRepository shopListProductRepository,
                        IProductRepository productRepository, IUserManager userManager,
                        IDbLogger<ShopList> logger) : IShopListService
{
    public async Task AddProductToShopList(int shopListId, int productId, int quantity = 1)
    {
        var shopList = await shopListRepository.GetShopListById(shopListId) ?? throw new ShopListNotFoundException($"Shop list with id {shopListId} not found.");
        var product = await productRepository.GetProductById(productId) ?? throw new ProductNotFoundException($"Product with id {productId} not found.");
        var dbShopListProduct = await shopListProductRepository.GetShopListProduct(shopListId, productId);
        if (dbShopListProduct != null)
        {
            int newQuantity = dbShopListProduct.Quantity + quantity;
            await shopListProductRepository.UpdateShopListProductQuantity(dbShopListProduct.Id, newQuantity);
        }
        else
        {
            var newShopListProduct = new ShopListProduct
            {
                ShopList = shopList,
                Product = product,
                Quantity = quantity
            };
            await shopListProductRepository.AddShopListProduct(newShopListProduct);
        }
        await logger.Log(Operation.Update, shopList);
    }

    public async Task CreateShopList(string userId, CreateShopListCommand cmd)
    {
        _ = userId ?? throw new ArgumentNullException();
        _ = cmd ?? throw new ArgumentNullException();
        var user = await userManager.FindByIdAsync(userId) ?? throw new UnauthorizedAccessException();
        var shopList = new ShopList
        {
            Name = cmd.Name,
            UserId = userId
        };
        await shopListRepository.AddShopList(shopList);
        await logger.Log(Operation.Create, shopList);
    }

    public async Task DeleteShopList(int shopListId)
    {
        bool result;
        var shopList = await shopListRepository.GetShopListById(shopListId) ?? throw new ShopListNotFoundException($"Shop list with id {shopListId} not found.");
        var shopListProducts = await shopListProductRepository.GetProductsForShopList(shopListId);
        foreach (var product in shopListProducts)
        {
            result = await shopListProductRepository.RemoveShopListProduct(shopListId, product.Id);
            if (!result) throw new ShopListProductNotFoundException($"Product with id {product.Id} not found in shop list with id {shopListId}");
        }
        result = await shopListRepository.RemoveShopList(shopListId);
        if (!result) throw new ShopListNotFoundException($"Shop list with id {shopListId} not found.");
        await logger.Log(Operation.Delete, shopList);
    }

    public async Task<ShopListResponse> GetShopListById(int shopListId)
    {
        var shopList = await shopListRepository.GetShopListById(shopListId) ?? throw new ShopListNotFoundException($"Shop list with id {shopListId} not found.");
        var shopListProducts = await shopListProductRepository.GetProductsForShopList(shopListId);
        var productViews = new List<ProductResponse>();
        foreach (var product in shopListProducts)
        {
            var shopListProductForQuantity = await shopListProductRepository.GetShopListProduct(shopListId, product.Id)
                ?? throw new ShopListProductNotFoundException($"Product with id {product.Id} not found in shop list with id {shopListId}");
            productViews.Add(new ProductResponse
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
        var shopListView = new ShopListResponse
        {
            Id = shopListId,
            Name = shopList.Name,
            OwnerId = shopList.UserId,
            Products = productViews
        };
        return shopListView;
    }

    public async Task RemoveProductFromShopList(int shopListId, int productId, int quantity = int.MaxValue)
    {
        var shopList = await shopListRepository.GetShopListById(shopListId) ?? throw new ShopListNotFoundException($"Shop list with id {shopListId} not found");
        var shopListProduct = await shopListProductRepository.GetShopListProduct(shopListId, productId)
            ?? throw new ShopListProductNotFoundException($"Product with id {productId} not found in shop list with id {shopListId}");
        bool result;
        if (quantity >= shopListProduct.Quantity)
            result = await shopListProductRepository.RemoveShopListProduct(shopListId, productId);
        else
        {
            int newQuantity = shopListProduct.Quantity - quantity;
            result = await shopListProductRepository.UpdateShopListProductQuantity(shopListProduct.Id, newQuantity);
        }
        if (!result) throw new Exception("Unexpected error occured");
        await logger.Log(Operation.Update, shopList);
    }

    public async Task UpdateShopList(int shopListId, UpdateShopListCommand cmd)
    {
        _ = cmd ?? throw new ArgumentNullException();
        var shopList = await shopListRepository.GetShopListById(shopListId)
                ?? throw new ShopListNotFoundException($"Shop list with id {shopListId} not found.");
        var updatedShopList = new ShopList
        {
            Name = cmd.Name,
            UserId = shopList.UserId
        };
        if (shopList == null) throw new ShopListNotFoundException($"Shop list with id {shopListId} not found.");
        var result = await shopListRepository.UpdateShopList(shopListId, updatedShopList);
        if (!result) throw new Exception("Unexpected error occured");
    }

    public async Task<ICollection<ShopListResponse>> GetShopListsForUser(string userId)
    {
        _ = userId ?? throw new ArgumentNullException();
        var user = await userManager.FindByIdAsync(userId) ?? throw new UnauthorizedAccessException();
        var shopLists = await shopListRepository.GetShopListsByUser(userId);
        var shopListViews = new List<ShopListResponse>();
        foreach (var shopList in shopLists)
        {
            var productViews = await GetProductsViewsForShopList(shopList);
            var shopListView = new ShopListResponse
            {
                Id = shopList.Id,
                Name = shopList.Name,
                OwnerId = shopList.UserId,
                Products = productViews
            };
            shopListViews.Add(shopListView);
        }
        return shopListViews;
    }

    private async Task<List<ProductResponse>> GetProductsViewsForShopList(ShopList shopList)
    {
        var shopListProducts = await shopListProductRepository.GetProductsForShopList(shopList.Id);
        var productViews = new List<ProductResponse>();
        foreach (var product in shopListProducts)
        {
            var shopListProductForQuantity = await shopListProductRepository.GetShopListProduct(shopList.Id, product.Id)
                ?? throw new ShopListProductNotFoundException($"Product with id {product.Id} not found in shop list with id {shopList.Id}");
            productViews.Add(new ProductResponse
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

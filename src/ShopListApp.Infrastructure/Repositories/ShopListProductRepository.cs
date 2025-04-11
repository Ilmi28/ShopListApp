using Microsoft.EntityFrameworkCore;
using ShopListApp.Core.Interfaces.IRepositories;
using ShopListApp.Core.Models;
using ShopListApp.Infrastructure.Database.Context;

namespace ShopListApp.Infrastructure.Repositories;

public class ShopListProductRepository(ShopListDbContext context) : IShopListProductRepository
{
    public async Task AddShopListProduct(ShopListProduct shopListProduct)
    {
        await context.ShopListProducts.AddAsync(shopListProduct);
        await context.SaveChangesAsync();
    }

    public async Task<bool> RemoveShopListProduct(int shopListId, int productId)
    {
        var shopListProduct = await context.ShopListProducts.FirstOrDefaultAsync(x => x.ShopList.Id == shopListId
            && x.Product.Id == productId);
        if (shopListProduct == null) 
            return false;
        shopListProduct.IsDeleted = true;
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<ICollection<Product>> GetProductsForShopList(int shopListId)
    {
        return await context.ShopListProducts.Where(x => x.ShopList.Id == shopListId)
            .Include(x => x.Product)
            .ThenInclude(x => x.Store)
            .Include(x => x.Product)
            .ThenInclude(x => x.Category)
            .Select(x => x.Product).ToListAsync();
    }

    public async Task<ShopListProduct?> GetShopListProduct(int shopListId, int productId)
    {
        return await context.ShopListProducts.FirstOrDefaultAsync(x => x.ShopList.Id == shopListId
        && x.Product.Id == productId);
    }

    public async Task<bool> UpdateShopListProductQuantity(int id, int newQuantity)
    {
        var shopListProduct = await context.ShopListProducts.FirstOrDefaultAsync(x => x.Id == id);
        if (shopListProduct == null)
            return false;
        shopListProduct.Quantity = newQuantity;
        await context.SaveChangesAsync();
        return true;
    }

}

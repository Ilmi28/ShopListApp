using Microsoft.EntityFrameworkCore;
using ShopListApp.Core.Interfaces.IRepositories;
using ShopListApp.Core.Models;
using ShopListApp.Infrastructure.Database.Context;

namespace ShopListApp.Infrastructure.Repositories;

public class ShopListRepository(ShopListDbContext context) : IShopListRepository
{
    public async Task AddShopList(ShopList shopList)
    {
        await context.ShopLists.AddAsync(shopList);
        await context.SaveChangesAsync();
    }

    public async Task<bool> RemoveShopList(int id)
    {
        var shopList = await context.ShopLists.FindAsync(id);
        if (shopList == null) 
            return false;
        shopList.IsDeleted = true;
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateShopList(int id, ShopList updatedShopList)
    {
        var shopList = await context.ShopLists.FindAsync(id);
        if (shopList == null) 
            return false;
        shopList.Name = updatedShopList.Name;
        shopList.UserId = updatedShopList.UserId;
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<ShopList?> GetShopListById(int id)
    {
        return await context.ShopLists.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<ICollection<ShopList>> GetShopListsByUser(string userId)
    {
        return await context.ShopLists.Where(x => x.UserId == userId).ToListAsync();
    }
}

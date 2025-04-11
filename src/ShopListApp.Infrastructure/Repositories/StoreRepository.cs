using Microsoft.EntityFrameworkCore;
using ShopListApp.Core.Interfaces.IRepositories;
using ShopListApp.Core.Models;
using ShopListApp.Infrastructure.Database.Context;

namespace ShopListApp.Infrastructure.Repositories;

public class StoreRepository(ShopListDbContext context) : IStoreRepository
{
    public async Task<Store?> GetStoreById(int id)
    {
        return await context.Stores.FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<ICollection<Store>> GetStores()
    {
        return await context.Stores.ToListAsync();
    }
}

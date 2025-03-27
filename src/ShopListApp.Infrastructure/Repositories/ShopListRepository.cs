using Microsoft.EntityFrameworkCore;
using ShopListApp.Exceptions;
using ShopListApp.Infrastructure.Database.Context;
using ShopListApp.Interfaces.IRepositories;
using ShopListApp.Models;

namespace ShopListApp.Repositories
{
    public class ShopListRepository : IShopListRepository
    {
        private readonly ShopListDbContext _context;
        public ShopListRepository(ShopListDbContext context)
        {
            _context = context;
        }

        public async Task AddShopList(ShopList shopList)
        {
            await _context.ShopLists.AddAsync(shopList);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> RemoveShopList(int id)
        {
            var shopList = await _context.ShopLists.FindAsync(id);
            if (shopList == null) 
                return false;
            shopList.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateShopList(int id, ShopList updatedShopList)
        {
            var shopList = await _context.ShopLists.FindAsync(id);
            if (shopList == null) 
                return false;
            shopList.Name = updatedShopList.Name;
            shopList.UserId = updatedShopList.UserId;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ShopList?> GetShopListById(int id)
        {
            return await _context.ShopLists.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<ICollection<ShopList>> GetShopListsByUser(string userId)
        {
            return await _context.ShopLists.Where(x => x.UserId == userId).ToListAsync();
        }
    }
}

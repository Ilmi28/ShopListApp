using ShopListApp.Database;
using ShopListApp.Models;

namespace ShopListApp.Repositories
{
    public class StoreRepository
    {
        private readonly ShopListDbContext _context;
        public StoreRepository(ShopListDbContext context)
        {
            _context = context;
        }

        public async Task<Store?> GetStoreById(int id)
        {
            return await _context.Stores.FindAsync(id);
        }
    }
}

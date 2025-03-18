using Microsoft.EntityFrameworkCore;
using ShopListApp.Database;
using ShopListApp.Interfaces.IRepositories;
using ShopListApp.Models;
using ShopListApp.ViewModels;
using System.Runtime.CompilerServices;

namespace ShopListApp.Repositories
{
    public class ShopListProductRepository : IShopListProductRepository
    {
        private readonly ShopListDbContext _context;
        public ShopListProductRepository(ShopListDbContext context)
        {
            _context = context;
        }

        public async Task AddShopListProduct(ShopListProduct shopListProduct)
        {
            await _context.ShopListProducts.AddAsync(shopListProduct);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> RemoveShopListProduct(int shopListId, int productId)
        {
            var shopListProduct = await _context.ShopListProducts.FirstOrDefaultAsync(x => x.ShopList.Id == shopListId
                && x.Product.Id == productId);
            if (shopListProduct == null) 
                return false;
            shopListProduct.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ICollection<Product>> GetProductsForShopList(int shopListId)
        {
            return await _context.ShopListProducts.Where(x => x.ShopList.Id == shopListId)
                .Select(x => x.Product).ToListAsync();
        }

    }
}

using Microsoft.EntityFrameworkCore;
using ShopListApp.Core.Interfaces.IRepositories;
using ShopListApp.Core.Models;
using ShopListApp.Infrastructure.Database.Context;

namespace ShopListApp.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private ShopListDbContext _context;
        public ProductRepository(ShopListDbContext context)
        {
            _context = context;
        }

        public async Task AddProduct(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> RemoveProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return false;
            product.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateProduct(int id, Product updatedProduct)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) 
                return false;
            product.Name = updatedProduct.Name;
            product.Price = updatedProduct.Price;
            product.Store = updatedProduct.Store;
            product.ImageUrl = updatedProduct.ImageUrl;
            product.Category = updatedProduct.Category;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Product?> GetProductById(int id)
        {
            return await _context.Products.Include(x => x.Store).Include(x => x.Category)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<ICollection<Product>> GetProductsByCategoryId(int categoryId)
        {
            var products = await _context.Products.Include(x => x.Store).Include(x => x.Category).Where(x => x.Category != null && x.Category.Id == categoryId).ToListAsync();
            return products;
        }

        public async Task<ICollection<Product>> GetProductsByStoreId(int storeId)
        {
            var products = await _context.Products.Include(x => x.Store).Include(x => x.Category).Where(x => x.Store.Id == storeId).ToListAsync();
            return products;
        }

        public async Task<ICollection<Product>> GetAllProducts()
        {
            var products = await _context.Products.Include(x => x.Store).Include(x => x.Category).ToListAsync();
            return products;
        }

        public async Task<Product?> GetProductByName(string name)
        {
            return await _context.Products.Include(x => x.Store).Include(x => x.Category).FirstOrDefaultAsync(x => x.Name == name);
        }
    }
}

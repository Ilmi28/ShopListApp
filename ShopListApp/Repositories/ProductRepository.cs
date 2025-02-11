using Microsoft.EntityFrameworkCore;
using ShopListApp.Database;
using ShopListApp.Exceptions;
using ShopListApp.Models;

namespace ShopListApp.Repositories
{
    public class ProductRepository
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
            _context.Products.Remove(product);
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
            product.StoreId = updatedProduct.StoreId;
            product.Store = updatedProduct.Store;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Product?> GetProductById(int id)
        {
            return await _context.Products.FindAsync(id);
        }
    }
}

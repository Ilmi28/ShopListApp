using ShopListApp.Database;
using ShopListApp.Enums;
using ShopListApp.Interfaces;
using ShopListApp.Models;

namespace ShopListApp.Loggers
{
    public class ProductLogger : IDbLogger<Product>
    {
        private readonly ShopListDbContext _context;
        public ProductLogger(ShopListDbContext context)
        {
            _context = context;
        } 
        public async Task Log(Operation operation, Product product)
        {
            var log = new ProductLog
            {
                ProductId = product.Id,
                Operation = operation
            };
            await _context.ProductLogs.AddAsync(log);
            await _context.SaveChangesAsync();
        }
    }
}

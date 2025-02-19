using ShopListApp.Database;
using ShopListApp.Enums;
using ShopListApp.Interfaces;
using ShopListApp.Models;

namespace ShopListApp.Loggers
{
    public class ShopListProductLogger : IDbLogger<ShopListProduct>
    {
        private readonly ShopListDbContext _context;
        public ShopListProductLogger(ShopListDbContext context)
        {
            _context = context;
        }
        public async Task Log(Operation operation, ShopListProduct loggedObject)
        {
            var log = new ShopListProductLog
            {
                ShopListId = loggedObject.ShopListId,
                ProductId = loggedObject.ProductId,
                Operation = operation
            };

            await _context.ShopListProductLogs.AddAsync(log);
            await _context.SaveChangesAsync();
        }
    }
}

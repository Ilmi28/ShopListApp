using ShopListApp.Database;
using ShopListApp.Enums;
using ShopListApp.Interfaces;
using ShopListApp.Models;

namespace ShopListApp.Loggers
{
    public class ShopListLogger : IDbLogger<ShopListProduct>
    {
        private readonly ShopListDbContext _context;
        public ShopListLogger(ShopListDbContext context)
        {
            _context = context;
        }
        public async Task Log(Operation operation, ShopListProduct loggedObject)
        {
            var log = new ShopListLog
            {
                ShopListId = loggedObject.Id,
                ProductId = loggedObject.ProductId,
                Operation = operation
            };

            await _context.ShopListLogs.AddAsync(log);
            await _context.SaveChangesAsync();
        }
    }
}

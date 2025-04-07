using ShopListApp.Core.Enums;
using ShopListApp.Core.Interfaces.ILogger;
using ShopListApp.Core.Models;
using ShopListApp.Infrastructure.Database.Context;

namespace ShopListApp.Infrastructure.Loggers
{
    public class ShopListLogger : IDbLogger<ShopList>
    {
        private readonly ShopListDbContext _context;
        public ShopListLogger(ShopListDbContext context)
        {
            _context = context;
        }
        public async Task Log(Operation operation, ShopList loggedObject)
        {
            var log = new ShopListLog
            {
                ShopListId = loggedObject.Id,
                Operation = operation
            };

            await _context.ShopListLogs.AddAsync(log);
            await _context.SaveChangesAsync();
        }
    }
}

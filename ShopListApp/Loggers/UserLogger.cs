using ShopListApp.Database;
using ShopListApp.Enums;
using ShopListApp.Interfaces;
using ShopListApp.Models;

namespace ShopListApp.Loggers
{
    public class UserLogger : IDbLogger<User>
    {
        private readonly ShopListDbContext _context;
        public UserLogger(ShopListDbContext context)
        {
            _context = context;
        }
        public async Task Log(Operation operation, User loggedObject)
        {
            var log = new UserLog
            {
                UserId = loggedObject.Id,
                Operation = operation
            };

            await _context.UserLogs.AddAsync(log);
            await _context.SaveChangesAsync();
        }
    }
}

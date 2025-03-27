using ShopListApp.Core.Dtos;
using ShopListApp.Core.Interfaces.ILogger;
using ShopListApp.Enums;
using ShopListApp.Infrastructure.Database.Context;
using ShopListApp.Models;

namespace ShopListApp.Loggers
{
    public class UserLogger : IDbLogger<UserDto>
    {
        private readonly ShopListDbContext _context;
        public UserLogger(ShopListDbContext context)
        {
            _context = context;
        }
        public async Task Log(Operation operation, UserDto loggedObject)
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

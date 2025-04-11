using ShopListApp.Core.Dtos;
using ShopListApp.Core.Enums;
using ShopListApp.Core.Interfaces.ILogger;
using ShopListApp.Core.Models;
using ShopListApp.Infrastructure.Database.Context;

namespace ShopListApp.Infrastructure.Loggers;

public class UserLogger(ShopListDbContext context) : IDbLogger<UserDto>
{
    public async Task Log(Operation operation, UserDto loggedObject)
    {
        var log = new UserLog
        {
            UserId = loggedObject.Id,
            Operation = operation
        };

        await context.UserLogs.AddAsync(log);
        await context.SaveChangesAsync();
    }
}

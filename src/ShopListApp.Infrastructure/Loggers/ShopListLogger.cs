using ShopListApp.Core.Enums;
using ShopListApp.Core.Interfaces.ILogger;
using ShopListApp.Core.Models;
using ShopListApp.Infrastructure.Database.Context;

namespace ShopListApp.Infrastructure.Loggers;

public class ShopListLogger(ShopListDbContext context) : IDbLogger<ShopList>
{
    public async Task Log(Operation operation, ShopList loggedObject)
    {
        var log = new ShopListLog
        {
            ShopListId = loggedObject.Id,
            Operation = operation
        };

        await context.ShopListLogs.AddAsync(log);
        await context.SaveChangesAsync();
    }
}

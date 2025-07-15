using Microsoft.EntityFrameworkCore;
using ShopListApp.Infrastructure.Database.Context;

namespace ShopListApp.API.ExtensionMethods
{
    public static class MigrationsExtensions
    {
        public static void ApplyMigrations(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ShopListDbContext>();
            if (dbContext.Database.IsRelational())
            {
                dbContext.Database.Migrate();
            }
        }
    }
}

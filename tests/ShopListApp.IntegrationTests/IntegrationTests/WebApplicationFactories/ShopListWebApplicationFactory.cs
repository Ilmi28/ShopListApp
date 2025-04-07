using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ShopListApp.API;
using ShopListApp.Infrastructure.Database.Context;
using ShopListApp.TestUtilities.Stubs;

namespace ShopListApp.IntegrationTests.IntegrationTests.WebApplicationFactories
{
    public class ShopListWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<DbContextOptions<ShopListDbContext>>();
                services.RemoveAll<ShopListDbContext>();

                services.AddDbContext<TestDbContext>(options =>
                                   options.UseInMemoryDatabase("ShopListTestDb"));

                services.AddScoped<ShopListDbContext, TestDbContext>();

            });
        }
    }
}

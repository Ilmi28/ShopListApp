using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ShopListApp.API;
using ShopListApp.Infrastructure.Database.Context;
using ShopListApp.TestUtilities.Stubs;

namespace ShopListApp.IntegrationTests.IntegrationTests.WebApplicationFactories;

public class AuthWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Environment.SetEnvironmentVariable("JWT_SECRET_KEY", "wJ6p+9nRJvBTqqkdmkz4zJ2OBt3nqu0YK0R5gTrgrYhVzwgNnFTH8XzSAQnEDDWX");
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<DbContextOptions>();
            services.RemoveAll<ShopListDbContext>();

            services.AddDbContext<TestDbContext>(options =>
                               options.UseInMemoryDatabase("AuthTestDb"));

            services.AddScoped<ShopListDbContext, TestDbContext>();

        });
    }
}

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Presentation;
using ShopListApp.Infrastructure.Database.Context;
using ShopListAppTests.Stubs;

namespace ShopListAppTests.IntegrationTests.WebApplicationFactories
{
    public class AuthWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<DbContextOptions<ShopListDbContext>>();
                services.RemoveAll<ShopListDbContext>();

                services.AddDbContext<TestDbContext>(options =>
                                   options.UseInMemoryDatabase("AuthTestDb"));

                services.AddScoped<ShopListDbContext, TestDbContext>();

            });
        }
    }
}


using ShopListApp.API.ExtensionMethods;
using ShopListApp.ExtensionMethods;

namespace Presentation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            InitializeApplication(args);
        }

        private static void InitializeApplication(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            ConfigureServices(builder);
            var app = builder.Build();
            ConfigureMiddleware(app);
            app.Run();
        }

        private static void ConfigureServices(WebApplicationBuilder builder)
        {
            builder.Services.AddRepositories();
            builder.Services.AddServices();
            builder.Services.AddLoggers();
            builder.Services.AddManagers();
            builder.Services.AddParsing();
            builder.Services.AddIdentityDbContext(builder.Configuration);
            builder.Services.AddJwtBearer(builder.Configuration);
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGenWithAuthorization();
            builder.Services.AddAuthorizationWithHandlers();
            builder.Services.AddStoreObserver();

        }

        private static void ConfigureMiddleware(WebApplication app)
        {
            app.UseCustomExceptionHandling();
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
        }
    }
}

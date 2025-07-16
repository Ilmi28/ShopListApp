using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ShopListApp.API.AppProblemDetails;
using ShopListApp.API.ExtensionMethods;
using ShopListApp.Core.Exceptions.BaseExceptions;

namespace ShopListApp.API;

public class Program
{
    public static void Main(string[] args)
    {
        InitializeApplication(args);
    }

    private static void InitializeApplication(string[] args)
    {
        DotNetEnv.Env.Load();
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
        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                context.Response.ContentType = "application/json";
                var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;


                ProblemDetails problemDetails = exception switch
                {
                    ConflictException => new ConflictProblemDetails(exception.Message),
                    NotFoundException => new NotFoundProblemDetails(exception.Message),
                    UnauthorizedAccessException => new UnauthorizedProblemDetails(exception.Message),
                    ArgumentNullException => new BadRequestProblemDetails(exception.Message),
                    InvalidOperationException => new BadRequestProblemDetails(exception.Message),
                    _ => new InternalServerErrorProblemDetails(exception!.Message)
                };
                context.Response.StatusCode = problemDetails.Status!.Value;
                await context.Response.WriteAsJsonAsync(problemDetails);
            });
        });
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.ApplyMigrations();
        }
        app.UseStatusCodePages();
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
    }
}

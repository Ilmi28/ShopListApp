
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using ShopListApp.Controllers;
using ShopListApp.CustomMiddleware;
using ShopListApp.Database;
using ShopListApp.ExtensionMethods;
using ShopListApp.Interfaces;
using ShopListApp.Managers;
using ShopListApp.Models;
using ShopListApp.Repositories;
using ShopListApp.Requirements;
using System.Text;
using static System.Net.WebRequestMethods;

namespace ShopListApp
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
            builder.Services.AddIdentityDbContext();
            builder.Services.AddJwtBearer();
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGenWithAuthorization();
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("ShopListOwnerPolicy", policy => policy.Requirements.Add(new ShopListOwnerRequirement()));
            });

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

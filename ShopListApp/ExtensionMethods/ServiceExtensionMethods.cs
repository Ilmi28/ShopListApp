using HtmlAgilityPack;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using ShopListApp.Database;
using ShopListApp.DataProviders;
using ShopListApp.Interfaces;
using ShopListApp.Interfaces.IRepositories;
using ShopListApp.Interfaces.IServices;
using ShopListApp.Loggers;
using ShopListApp.Managers;
using ShopListApp.Models;
using ShopListApp.Repositories;
using ShopListApp.Services;
using System.Text;

namespace ShopListApp.ExtensionMethods
{
    public static class ServiceExtensionMethods
    {
        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddTransient<IProductRepository, ProductRepository>();
            services.AddTransient<ICategoryRepository, CategoryRepository>();
            services.AddTransient<IShopListProductRepository, ShopListProductRepository>();
            services.AddTransient<IShopListRepository, ShopListRepository>();
            services.AddTransient<IStoreRepository, StoreRepository>();
            services.AddTransient<ITokenRepository, TokenRepository>();
        }

        public static void AddServices(this IServiceCollection services)
        {
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IAuthService, AuthService>();
            services.AddTransient<IStoreService, StoreService>();
            services.AddTransient<IProductService, ProductService>();

        }

        public static void AddLoggers(this IServiceCollection services)
        {
            services.AddTransient<IDbLogger<User>, UserLogger>();
            services.AddTransient<IDbLogger<ShopListProduct>, ShopListProductLogger>();
        }

        public static void AddManagers(this IServiceCollection services)
        {
            services.AddTransient<ITokenManager, JwtTokenManager>();
        }

        public static void AddParsing(this IServiceCollection services)
        {
            services.AddTransient<IHtmlFetcher<HtmlNode, HtmlDocument>, HAPHtmlFetcher>();
            services.AddHttpClient();
        }

        public static void AddIdentityDbContext(this IServiceCollection services)
        {
            var configuration = services.BuildServiceProvider().GetService<IConfiguration>();
            var connString = Environment.GetEnvironmentVariable("ShopListAppConnectionString") 
                ?? configuration!.GetConnectionString("DefaultConnection")
                ?? String.Empty;
            services.AddDbContext<ShopListDbContext>(options =>
            {
                options.UseSqlServer(connString);
            });
            services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<ShopListDbContext>();
        }

        public static void AddJwtBearer(this IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                var configuration = services.BuildServiceProvider().GetService<IConfiguration>();
                var tokenConfiguration = configuration!.GetSection("TokenConfiguration");
                string secretKey = Environment.GetEnvironmentVariable("JwtSecretKey")
                    ?? tokenConfiguration.GetValue<string>("SecretKey")
                    ?? String.Empty;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = tokenConfiguration.GetValue<string>("Issuer"),
                    ValidAudience = tokenConfiguration.GetValue<string>("Audience"),
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
                };
            });
        }

        public static void AddSwaggerGenWithAuthorization(this IServiceCollection services)
        {
            services.AddSwaggerGen(x =>
            {
                var security = new OpenApiSecurityScheme
                {
                    Name = HeaderNames.Authorization,
                    Type = SecuritySchemeType.ApiKey,
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header",
                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };

                x.AddSecurityDefinition(security.Reference.Id, security);
                x.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {{security, Array.Empty<string>()}});
            });
        }
    }
}

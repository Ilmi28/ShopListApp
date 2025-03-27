using HtmlAgilityPack;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using ShopListApp.Application.AuthorizationPolicies.RequirementHandlers;
using ShopListApp.Application.AuthorizationPolicies.Requirements;
using ShopListApp.Application.HtmlFetchers;
using ShopListApp.Core.Dtos;
using ShopListApp.Core.Interfaces;
using ShopListApp.Core.Interfaces.Identity;
using ShopListApp.Core.Interfaces.ILogger;
using ShopListApp.Core.Interfaces.Parsing;
using ShopListApp.Core.Interfaces.StoreObserver;
using ShopListApp.Infrastructure.Database.Context;
using ShopListApp.Infrastructure.Database.Identity;
using ShopListApp.Interfaces;
using ShopListApp.Interfaces.IRepositories;
using ShopListApp.Interfaces.IServices;
using ShopListApp.Loggers;
using ShopListApp.Managers;
using ShopListApp.Models;
using ShopListApp.Repositories;
using ShopListApp.Services;
using ShopListApp.StoreObserver;
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
            services.AddTransient<IShopListService, ShopListService>();

        }

        public static void AddLoggers(this IServiceCollection services)
        {
            services.AddTransient<IDbLogger<UserDto>, UserLogger>();
            services.AddTransient<IDbLogger<ShopList>, ShopListLogger>();
        }

        public static void AddManagers(this IServiceCollection services)
        {
            services.AddTransient<ITokenManager, JwtTokenManager>();
            services.AddTransient<IUserManager, UserManager>();
        }

        public static void AddParsing(this IServiceCollection services)
        {
            services.AddTransient<IHtmlFetcher<HtmlNode, HtmlDocument>, HAPHtmlFetcher>();
            services.AddHttpClient();
        }

        public static void AddStoreObserver(this IServiceCollection services)
        {
            services.AddTransient<IStorePublisher, StorePublisher>();
        }

        public static void AddIdentityDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            var connString = Environment.GetEnvironmentVariable("ShopListAppConnectionString") 
                ?? configuration!.GetConnectionString("DefaultConnection")
                ?? String.Empty;
            services.AddDbContext<ShopListDbContext>(options =>
            {
                options.UseSqlServer(connString);
            });
            services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<ShopListDbContext>();
        }

        public static void AddJwtBearer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
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

        public static void AddAuthorizationWithHandlers(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy("ShopListOwnerPolicy", policy => policy.Requirements.Add(new ShopListOwnerRequirement()));
            });
            services.AddScoped<IAuthorizationHandler, ShopListOwnerAuthorizationHandler>();
        }
    }
}

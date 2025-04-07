using ShopListApp.API.Middleware;

namespace ShopListApp.API.ExtensionMethods
{
    public static class MiddlewareExtensionMethods
    {
        public static IApplicationBuilder UseCustomExceptionHandling(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionHandlerMiddleware>();
        }
    }
}

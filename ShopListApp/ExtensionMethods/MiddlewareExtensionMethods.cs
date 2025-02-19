namespace ShopListApp.ExtensionMethods
{
    public static class MiddlewareExtensionMethods
    {
        public static IApplicationBuilder UseCustomExceptionHandling(this WebApplication app)
        {
            return app.UseMiddleware<CustomMiddleware.ExceptionHandlerMiddleware>();
        }
    }
}

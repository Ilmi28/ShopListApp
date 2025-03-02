namespace ShopListApp.ExtensionMethods
{
    public static class MiddlewareExtensionMethods
    {
        public static IApplicationBuilder UseCustomExceptionHandling(this IApplicationBuilder app)
        {
            return app.UseMiddleware<CustomMiddleware.ExceptionHandlerMiddleware>();
        }
    }
}

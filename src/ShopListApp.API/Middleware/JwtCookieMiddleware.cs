namespace ShopListApp.API.Middleware;

public class JwtCookieMiddleware(RequestDelegate next, string cookieName = "accessToken")
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.ContainsKey("Authorization"))
        {
            var token = context.Request.Cookies[cookieName];
            if (!string.IsNullOrEmpty(token))
            {
                context.Request.Headers.Add("Authorization", $"Bearer {token}");
            }
        }

        await next(context);
    }
}
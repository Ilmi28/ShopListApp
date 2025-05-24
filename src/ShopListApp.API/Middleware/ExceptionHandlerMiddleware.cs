using ShopListApp.Core.Exceptions;

namespace ShopListApp.API.Middleware;

public class ExceptionHandlerMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptions(context, ex);
        }
    }

    public async Task HandleExceptions(HttpContext context, Exception ex)
    {
        switch (ex)
        {
            case InvalidOperationException:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync(ex.Message);
                return;
            case UnauthorizedAccessException:
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized access.");
                return;
            case ArgumentNullException:
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("Invalid input.");
                return;
            case UserWithEmailAlreadyExistsException:
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("User with this email already exists.");
                return;
            case UserWithUserNameAlreadyExistsException:
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("User with this username already exists.");
                return;
            case UserAlreadyExistsException:
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("User already exists.");
                return;
            case FetchingErrorException:
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync("Error occurred while fetching data.");
                return;
            case CategoryNotFoundException:
                context.Response.StatusCode = 404;
                await context.Response.WriteAsync("Category not found.");
                return;
            case StoreNotFoundException:
                context.Response.StatusCode = 404;
                await context.Response.WriteAsync("Store not found.");
                return;
            case ProductNotFoundException:
                context.Response.StatusCode = 404;
                await context.Response.WriteAsync("Product not found.");
                return;
            case ShopListNotFoundException:
                context.Response.StatusCode = 404;
                await context.Response.WriteAsync("Shopping list not found.");
                return;
            case ShopListProductNotFoundException:
                context.Response.StatusCode = 404;
                await context.Response.WriteAsync("Product is not in shopping list");
                return;

        }
        context.Response.StatusCode = 500;
        await context.Response.WriteAsync("An error occurred. Please try again later.");
    }
}

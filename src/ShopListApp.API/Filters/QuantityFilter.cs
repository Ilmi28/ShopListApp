using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ShopListApp.Filters
{
    public class QuantityFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {   
            if (context.ActionArguments.TryGetValue("quantity", out var value) && value is int)
            {
                int quantity = (int)value;
                if (quantity < 1)
                {
                    context.Result = new BadRequestObjectResult("Quantity must be greater than 0.");
                }
            }
        }
    }
}

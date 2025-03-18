using Microsoft.AspNetCore.Authorization;
using ShopListApp.Models;
using ShopListApp.Requirements;
using System.Security.Claims;

namespace ShopListApp.RequirementHandlers
{
    public class ShopListOwnerAuthorizationHandler : AuthorizationHandler<ShopListOwnerRequirement, ShopList>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ShopListOwnerRequirement requirement, ShopList resource)
        {
            if (context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value == resource.User.Id)
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}

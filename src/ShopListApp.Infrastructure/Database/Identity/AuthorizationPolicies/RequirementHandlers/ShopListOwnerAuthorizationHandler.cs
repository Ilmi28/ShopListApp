using Microsoft.AspNetCore.Authorization;
using ShopListApp.Core.Responses;
using ShopListApp.Infrastructure.Database.Identity.AuthorizationPolicies.Requirements;
using System.Security.Claims;

namespace ShopListApp.Infrastructure.Database.Identity.AuthorizationPolicies.RequirementHandlers;

public class ShopListOwnerAuthorizationHandler : AuthorizationHandler<ShopListOwnerRequirement, ShopListResponse>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ShopListOwnerRequirement requirement, ShopListResponse resource)
    {
        if (context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value == resource.OwnerId)
        {
            context.Succeed(requirement);
        }
        return Task.CompletedTask;
    }
}

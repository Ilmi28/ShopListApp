using Microsoft.AspNetCore.Authorization;
using ShopListApp.Application.AuthorizationPolicies.Requirements;
using ShopListApp.Models;
using ShopListApp.ViewModels;
using System.Security.Claims;

namespace ShopListApp.Application.AuthorizationPolicies.RequirementHandlers
{
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
}

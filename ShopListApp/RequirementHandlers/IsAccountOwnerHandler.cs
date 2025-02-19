using Microsoft.AspNetCore.Authorization;
using ShopListApp.Requirements;

namespace ShopListApp.RequirementHandlers
{
    public class IsAccountOwnerHandler : AuthorizationHandler<IsAccountOwnerRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public IsAccountOwnerHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IsAccountOwnerRequirement requirement)
        {
            throw new NotImplementedException();
        }
    }
}

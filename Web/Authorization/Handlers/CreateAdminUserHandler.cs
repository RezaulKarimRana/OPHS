using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Infrastructure.Cache;
using Infrastructure.Cache.Contracts;
using Web.Authorization.Requirements;

namespace Web.Authorization.Handlers
{
    public class CreateAdminUserHandler : AuthorizationHandler<CreateAdminUserRequirement>
    {
        private readonly ICacheProvider _cacheProvider;

        public CreateAdminUserHandler(ICacheProvider cacheProvider)
        {
            _cacheProvider = cacheProvider;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, CreateAdminUserRequirement requirement)
        {
            _cacheProvider.TryGet(CacheConstants.AdminUserExists, out bool? requiresAdminUser);
            if (requiresAdminUser.HasValue &&
                requiresAdminUser.Value == true)
            {
                context.Succeed(requirement);
            }
        }
    }
}

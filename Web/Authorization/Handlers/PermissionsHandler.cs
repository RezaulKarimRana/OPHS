﻿using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Threading.Tasks;
using Web.Authorization.Requirements;
using Services.Managers.Contracts;

namespace Web.Authorization.Handlers
{
    public class PermissionsHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly ISessionManager _sessionManager;

        public PermissionsHandler(ISessionManager sessionManager)
        {
            _sessionManager = sessionManager;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            var permissions = await _sessionManager.GetPermissions();
            if (permissions != null && 
                permissions.Any(p => requirement.Permissions.Contains(p.Key)))
            {
                context.Succeed(requirement);
            }
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Threading.Tasks;
using AMS.Services.Managers.Contracts;

namespace AMS.Web.Filters
{
    public class PermissionRequirementFilter : IAsyncAuthorizationFilter
    {
        private readonly string _key;
        private readonly ISessionManager _sessionManager;

        public PermissionRequirementFilter(string key, ISessionManager sessionManager)
        {
            _key = key;
            _sessionManager = sessionManager;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var permissions = await _sessionManager.GetPermissions();
            if (permissions == null || !permissions.Any(c => c.Key == _key))
            {
                context.Result = new ForbidResult();
            }
        }
    }
}

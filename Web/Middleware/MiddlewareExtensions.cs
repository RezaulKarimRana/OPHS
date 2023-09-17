using Microsoft.AspNetCore.Builder;

namespace AMS.Web.Middleware
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseAdminCreationMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AdminCreationMiddleware>();
        }
    }
}

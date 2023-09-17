using AMS.Web.Filters;
using Microsoft.AspNetCore.Mvc;

namespace AMS.Web.Attributes
{
    public class AuthorizePermissionAttribute : TypeFilterAttribute
    {
        public AuthorizePermissionAttribute(string key) : base(typeof(PermissionRequirementFilter))
        {
            Arguments = new object[] { key };
        }
    }
}
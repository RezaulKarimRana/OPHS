using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AMS.Web.Pages
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class Error404Model : BaseErrorPageModel
    {
        public Error404Model(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor) { }

        public void OnGet()
        {

        }
    }
}

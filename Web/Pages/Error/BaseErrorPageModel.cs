using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace Web.Pages
{
    public class BaseErrorPageModel : BasePageModel
    {
        public string OriginalPath { get; set; }

        public BaseErrorPageModel(IHttpContextAccessor httpContextAccessor)
        {
            var feature = httpContextAccessor.HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
            OriginalPath = feature?.OriginalPath + feature?.OriginalQueryString;
        }
    }
}

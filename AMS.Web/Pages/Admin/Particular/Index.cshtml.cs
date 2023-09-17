using System.Threading.Tasks;
using AMS.Services.Admin.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AMS.Web.Pages.Admin.Particular
{
    public class IndexModel : PageModel
    {
        private readonly IParticularService _particularService;

        public IndexModel(IParticularService particularService)
        {
            _particularService = particularService;
        }

        public void OnGet()
        {
        }

        public async Task<JsonResult> OnGetData()
        {
            var response = await _particularService.GetParticulars();
            return new JsonResult(response);
        }
    }
}

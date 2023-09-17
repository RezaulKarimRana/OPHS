using AMS.Services.Admin.Contracts;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AMS.Web.Controllers
{
    [Route("[controller]/[action]")]
    public class DistrictController : Controller
    {
        private readonly IDistService _distService;

        public DistrictController(IDistService distService)
        {
            _distService = distService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDistricts()
        {
            var response = await _distService.GetAllDistrict();
            return new JsonResult(response);
        }
    }
}

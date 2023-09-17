using AMS.Services.Admin.Contracts;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AMS.Web.Controllers
{
    [Route("[controller]/[action]")]
    public class ThanaController : Controller
    {
        private readonly IThanaService _thanaService;

        public ThanaController(IThanaService thanaService)
        {
            _thanaService = thanaService;
        }

        [HttpGet]
        public async Task<IActionResult> GetThanaByDistrictId(int distId)
        {
            var response = await _thanaService.getThanaByDistIDResponse(distId);
            return new JsonResult(response);
        }
    }
}

using AMS.Services.Admin.Contracts;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AMS.Web.Controllers
{
    [Route("[controller]/[action]")]
    public class ParticularController : BaseController
    {
        private readonly IParticularService _particularService;

        public ParticularController(IParticularService particularService)
        {
            _particularService = particularService;
        }

        [HttpGet]
        public async Task<IActionResult> GetParticulars()
        {
            var response = await _particularService.GetParticulars();
            return new JsonResult(response);
        }

    }
}

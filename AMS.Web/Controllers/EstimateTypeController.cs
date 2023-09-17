using AMS.Services.Admin.Contracts;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AMS.Web.Controllers
{
    [Route("[controller]/[action]")]
    public class EstimateTypeController : BaseController
    {
        private readonly IEstimateTypeService _estimatetypeService;

        public EstimateTypeController(IEstimateTypeService estimatetypeService)
        {
            _estimatetypeService = estimatetypeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEstimateTypes()
        {
            var response = await _estimatetypeService.LoadAllTheEstimateTypes();
            return new JsonResult(response);
        }
        [HttpGet]
        public async Task<IActionResult> GetAllDropdownItemByDropdownNameAndEstimateType(string dropdownName, int estimateType)
        {
            var response = await _estimatetypeService.LoadAllDropdownItemByDropdownNameAndEstimationType(dropdownName, estimateType);
            return new JsonResult(response);
        }
    }
}

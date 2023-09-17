using AMS.Services.Admin.Contracts;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AMS.Web.Controllers
{
    [Route("[controller]/[action]")]
    public class ItemCategoryController : BaseController
    {
        private readonly IItemCategoryService _itemCategoryService;

        public ItemCategoryController(IItemCategoryService itemCategoryService)
        {
            _itemCategoryService = itemCategoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllItemCategoriesBYparticularId(int particularId)
        {
            var response = await _itemCategoryService.GetItemesByParticularName(particularId);
            return new JsonResult(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllItemCategories()
        {
            var response = await _itemCategoryService.GetItemes();
            return new JsonResult(response);
        }
    }
}

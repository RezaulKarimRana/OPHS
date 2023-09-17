using AMS.Models.ViewModel;
using AMS.Services.Admin.Contracts;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AMS.Web.Controllers
{
    [Route("[controller]/[action]")]
    public class ItemController : BaseController
    {
        private readonly IItemService _itemService;

        public ItemController(IItemService itemService)
        {
            _itemService = itemService;
        }

        [HttpGet]
        public async Task<IActionResult> ItemHome()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetItemUnitDetails(int itemCategoryId)
        {
            try
            {
                var response = await _itemService.GetItemUnitDetails(itemCategoryId);
                return new JsonResult(response);
            }
            catch (System.Exception)
            {

                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetItemAnditsDetailByItemCode(string code)
        {
            try
            {
                var response = await _itemService.GetItemDetailsByItemCodeService(code);
                return new JsonResult(response);
            }
            catch (System.Exception)
            {

                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetItemAnditsDetailByItemCodes(ItemSearchDTO dto)
        {
            try
            {
                var response = await _itemService.GetItemDetailsByItemCodesService(dto.Codes ?? "");
                return new JsonResult(response);
            }
            catch (System.Exception)
            {
                throw;
            }
        }
    }
}

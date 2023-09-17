using AMS.Models.ViewModel;
using AMS.Services.Admin.Contracts;
using AMS.Services.Managers.Contracts;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AMS.Web.Controllers
{
    [Route("[controller]/[action]")]
    public class AdminSetupController : BaseController
    {
        private readonly IAdminSetUpService _service;
        private readonly ISessionManager _sessionManager;

        public AdminSetupController(IAdminSetUpService service, ISessionManager sessionManager)
        {
            _service = service;
            _sessionManager = sessionManager;
        }

        [HttpGet]
        public async Task<IActionResult> UserDepartmentUpdate()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> ApproverRoleModification()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> ItemCategoryCreate()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> ParticularCreate()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> AmsItemCreate()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> ItemCreate()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> ApproverModification()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> UpdateRequestStatus()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetItemInitData()
        {
            var response = await _service.GetItemInitData();
            return new JsonResult(response);
        }
        [HttpPost]
        public async Task<IActionResult> SaveItem(ItemSaveModel model)
        {
            try
            {
                var user = await _sessionManager.GetUser();
                if (user == null) return RedirectToHome();

                model.CreatedById = user.Id;
                var result = await _service.SaveItem(model);
                return Ok(new { Success = true, Message = "Item Creation Success"});
            }
            catch(Exception ex)
            {
                return Ok(new { Success = false, Message = "Item Creation Failed" });
            }
        }
        [HttpPost]
        public async Task<IActionResult> UpdateApproverRole(ApproverRoleUpdateModel model)
        {
            try
            {
                var user = await _sessionManager.GetUser();
                if (user == null) return RedirectToHome();

                var result = await _service.UpdateApproverRole(model);
                return Ok(new { Success = true, Message = "Success!!"});
            }
            catch(Exception ex)
            {
                return Ok(new { Success = false, Message = "Failed!!" });
            }
        }
        [HttpPost]
        public async Task<IActionResult> AMSSaveItem(ItemSaveModel model)
        {
            try
            {
                var user = await _sessionManager.GetUser();
                if (user == null) return RedirectToHome();

                model.CreatedById = user.Id;
                var result = await _service.AMSSaveItem(model);
                return Ok(new { Success = true, Message = "Item Creation Success"});
            }
            catch(Exception ex)
            {
                return Ok(new { Success = false, Message = "Item Creation Failed" });
            }
        }
        [HttpPost]
        public async Task<IActionResult> SaveParticular(NameModel model)
        {
            try
            {
                var user = await _sessionManager.GetUser();
                if (user == null) return RedirectToHome();

                model.CreatedById = user.Id;
                var result = await _service.SaveParticular(model);
                return Ok(new { Success = true, Message = "Particular Creation Success"});
            }
            catch(Exception ex)
            {
                return Ok(new { Success = false, Message = "Particular Creation Failed" });
            }
        }
        [HttpPost]
        public async Task<IActionResult> SaveItemCategory(ItemCategoryModel model)
        {
            try
            {
                var user = await _sessionManager.GetUser();
                if (user == null) return RedirectToHome();

                model.CreatedById = user.Id;
                var result = await _service.SaveItemCategory(model);
                return Ok(new { Success = true, Message = "Particular Creation Success"});
            }
            catch(Exception ex)
            {
                return Ok(new { Success = false, Message = "Particular Creation Failed" });
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetItemCategoryByParticularId(int particularId)
        {
            try
            {
                var response = await _service.GetItemCategoryByParticularId(particularId);
                return Ok(response);
            }
            catch (Exception e)
            {
                return Ok(new { Success = false, Message = "Particular Creation Failed" });
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetAllApprover(int moduleId, string requestNo)
        {
            var response = await _service.GetAllApprover(moduleId,requestNo);
            return new JsonResult(response);
        }
        [HttpPost]
        public async Task<ActionResult> UpdateApproverModification(ApproverModificationUpdateModel model)
        {
            var approverModel = JsonConvert.DeserializeObject<List<ApproverModel>>(model.Items);
            model.Approvers = approverModel;
            var result = await _service.UpdateApproverModification(model);
            return Json(result);
        }
        [HttpPost]
        public async Task<ActionResult> UpdateRequestStatus(StatusUpdateModel model)
        {
            var result = await _service.UpdateRequestStatus(model);
            return Json(result);
        }
        [HttpPost]
        public async Task<ActionResult> UpdateUserDepartment(UserDepartmentUpdateModel model)
        {
            var result = await _service.UpdateUserDepartment(model);
            return Json(result);
        }
    }
}

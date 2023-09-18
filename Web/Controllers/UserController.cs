using Models.ViewModel;
using AMS.Services.Admin.Contracts;
using AMS.Services.Managers.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace AMS.Web.Controllers
{
    [Route("[controller]/[action]")]
    public class UserController : BaseController
    {
        private readonly ILogger<UserController> _logger;
        private readonly ISessionManager _sessionManager;
        private readonly IUserService _userService;

        public UserController(IUserService userService, ILoggerFactory loggerFactory, ISessionManager sessionManager)
        {
            _userService = userService;
            _logger = loggerFactory.CreateLogger<UserController>();
            _sessionManager = sessionManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsersWithDepartmentName()
        {
            var response = await _userService.GetUsersWithDepartmentService();
            return new JsonResult(response);
        }

        [HttpPost]
        public async Task<IActionResult> GetUserByUserId()
        {
            try
            {   //
                var sessionUser = await _sessionManager.GetUser();
                CheckDepartmentForCRA response = new CheckDepartmentForCRA
                {
                    DeptId = sessionUser.Department_Id
                };
                if (response.DeptId == 7)
                    response.isCRA = true;
                else
                    response.isCRA = false;
                return new JsonResult(response);
            }
            catch (System.Exception e)
            {
                _logger.LogError(e, e.Message);
                return Json(0);
            }
        }
    }
}

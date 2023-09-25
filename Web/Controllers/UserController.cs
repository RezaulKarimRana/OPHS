using Services.Managers.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Web.Controllers
{
    [Route("[controller]/[action]")]
    public class UserController : BaseController
    {
        private readonly ILogger<UserController> _logger;
        private readonly ISessionManager _sessionManager;

        public UserController(ILoggerFactory loggerFactory, ISessionManager sessionManager)
        {
            _logger = loggerFactory.CreateLogger<UserController>();
            _sessionManager = sessionManager;
        }

        [HttpPost]
        public async Task<IActionResult> GetUserByUserId()
        {
            try
            {
                var sessionUser = await _sessionManager.GetUser();
                return new JsonResult(sessionUser);
            }
            catch (System.Exception e)
            {
                _logger.LogError(e, e.Message);
                return Json(0);
            }
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AMS.Web.Controllers
{
    [Route("[controller]/[action]")]
    public class SessionController : BaseController
    {
        #region Instance Fields


        #endregion

        #region Constructors

        public SessionController()
        {
        }

        #endregion

        #region Public Methods

        [HttpGet]
        public async Task<IActionResult> Heartbeat()
        {
            return Ok();
        }

        #endregion
    }
}

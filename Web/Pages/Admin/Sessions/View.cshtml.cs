using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Models.DomainModels;
using Models.ServiceModels.Admin.Sessions;
using Services.Admin.Contracts;

namespace Web.Pages
{
    public class ViewSessionModel : BasePageModel
    {
        #region Private Fields

        private readonly ISessionService _sessionService;

        #endregion

        #region Properties

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }


        public UserEntity? UserEntity { get; set; }

        public SessionEntity? SessionEntity { get; set; }

        #endregion

        #region Constructors

        public ViewSessionModel(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        #endregion

        public async Task<IActionResult> OnGet()
        {
            var response = await _sessionService.GetSession(new GetSessionRequest()
            {
                Id = Id
            });

            if (!response.IsSuccessful)
            {
                return NotFound();
            }

            UserEntity = response.User;
            SessionEntity = response.Session;

            return Page();
        }

        public async Task<JsonResult> OnGetData(int id)
        {
            var response = await _sessionService.GetSessionLogs(new GetSessionLogsRequest()
            {
                Session_Id = id
            });

            return new JsonResult(response);
        }
    }
}

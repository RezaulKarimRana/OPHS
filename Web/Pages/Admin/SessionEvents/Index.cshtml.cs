using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;
using Models.DomainModels;
using Services.Admin.Contracts;

namespace Web.Pages
{
    public class ManageSessionEventsModel : PageModel
    {
        #region Private Fields

        private readonly ISessionService _sessionService;

        #endregion

        #region Properties

        public List<SessionEventEntity> SessionEvents { get; set; }

        #endregion

        #region Constructors

        public ManageSessionEventsModel(ISessionService sessionService)
        {
            _sessionService = sessionService;
            SessionEvents = new List<SessionEventEntity>();
        }

        #endregion

        public async Task OnGet()
        {
            var response = await _sessionService.GetSessionEvents();
            SessionEvents = response.SessionEvents;
        }
    }
}

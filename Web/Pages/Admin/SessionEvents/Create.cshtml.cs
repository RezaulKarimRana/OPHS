using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Models.ServiceModels.Admin.SessionEvents;
using Services.Admin.Contracts;

namespace Web.Pages
{
    public class CreateSessionEventModel : BasePageModel
    {
        #region Private Fields

        private readonly ISessionService _sessionService;

        #endregion

        #region Properties

        [BindProperty]
        public CreateSessionEventRequest FormData { get; set; }

        #endregion

        #region Constructors

        public CreateSessionEventModel(ISessionService sessionService)
        {
            _sessionService = sessionService;
            FormData = new CreateSessionEventRequest();
        }

        #endregion

        public async Task OnGet()
        {
        }

        public async Task<IActionResult> OnPost()
        {
            if (ModelState.IsValid)
            {
                var response = await _sessionService.CreateSessionEvent(FormData);
                if (response.IsSuccessful)
                {
                    AddNotifications(response);
                    return RedirectToPage("/Admin/SessionEvents/Index");
                }
                AddFormErrors(response);
            }
            return Page();
        }
    }
}

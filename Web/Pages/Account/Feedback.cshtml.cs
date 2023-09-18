using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Models.ServiceModels.Account;
using AMS.Services.Contracts;
using Models.ServiceModels.Dashboard;

namespace AMS.Web.Pages
{
    public class FeedbackModel : BasePageModel
    {
        #region Private Fields

        private readonly IAccountService _service;
        private readonly IDashboardService _dashboardService;
        #endregion

        #region Properties

        [BindProperty]
        public SendFeedbackRequest FormData { get; set; }

        #endregion

        #region Constructors

        public FeedbackModel(IAccountService service, IDashboardService dashboardService)
        {
            _service = service;
            _dashboardService = dashboardService;
            FormData = new SendFeedbackRequest();
        }

        #endregion


        public async Task OnGet() 
        {
            var responseForNav = await _dashboardService.GetNavBarCount();
            if (responseForNav.IsSuccessful)
            {
                ViewData["CountPendingObject"] = new GetCountForAllPendingParkingForNavService
                {
                    TotalDraftParking = responseForNav.TotalDraftParking,
                    TotalCompletedParking = responseForNav.TotalCompletedParking,
                    TotalPendingApprovalParking = responseForNav.TotalPendingApprovalParking,
                    TotalRollbackParking = responseForNav.TotalRollbackParking
                };
            }
        }

        public async Task<IActionResult> OnPost()
        {
            if (ModelState.IsValid)
            {
                var response = await _service.SendFeedback(FormData);
                if (response.IsSuccessful)
                {
                    AddNotifications(response);
                    return RedirectToHome();
                }
                AddFormErrors(response);
            }
            var responseForNav = await _dashboardService.GetNavBarCount();
            if (responseForNav.IsSuccessful)
            {
                ViewData["CountPendingObject"] = new GetCountForAllPendingParkingForNavService
                {
                    TotalDraftParking = responseForNav.TotalDraftParking,
                    TotalCompletedParking = responseForNav.TotalCompletedParking,
                    TotalPendingApprovalParking = responseForNav.TotalPendingApprovalParking,
                    TotalRollbackParking = responseForNav.TotalRollbackParking
                };
            }
            return Page();
        }
    }
}

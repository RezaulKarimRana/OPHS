using System.Threading.Tasks;
using Models.ServiceModels.Dashboard;
using Services.Contracts;
using Services.Managers.Contracts;

namespace Web.Pages
{
    public class IndexModel : BasePageModel
    {
        private readonly IDashboardService _dashboardService;
        private readonly ISessionManager _sessionManager;

        public IndexModel(IDashboardService dashboardService, ISessionManager sessionManager)
        {
            _dashboardService = dashboardService;
            _sessionManager = sessionManager;
        }

        public async Task OnGet()
        {
            var sessionUser = await _sessionManager.GetUser();
            if (sessionUser == null)
            {
                Response.Redirect("/Account/Login");
            }
            else
            {
                var response = await _dashboardService.GetIndexDashBoard();
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
                AddNotifications(response);
            }
        }
    }
}

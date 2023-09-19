using System.Threading.Tasks;
using Services.Contracts;

namespace Web.Pages.Dashboard
{
    public class dashboardModel : BasePageModel
    {
        #region Private Fields

        private readonly IDashboardService _dashboardService;

        #endregion

        #region Properties

        public int TotalSessions { get; set; }

        public int TotalUsers { get; set; }

        public int TotalRoles { get; set; }

        public int TotalConfigItems { get; set; }
        public int TotalRunningBudget { get; private set; }
        public int TotalCompletedBudget { get; private set; }
        public decimal TotalAmount { get; private set; }

        #endregion

        #region Constructors

        public dashboardModel(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        #endregion

        public async Task OnGet()
        {
            var response = await _dashboardService.GetIndexDashBoard();
            if (response.IsSuccessful)
            {
                TotalUsers = response.TotalUsers;
                TotalSessions = response.TotalSessions;
                TotalRoles = response.TotalRoles;
                TotalConfigItems = response.TotalConfigItems;
                TotalRunningBudget = response.TotalRunningBudget;
                TotalCompletedBudget = response.TotalCompletedBudget;
                TotalAmount = response.TotalAmount;
            }
            AddNotifications(response);
        }
    }
}

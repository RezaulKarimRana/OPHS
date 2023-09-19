using System.Collections.Generic;
using System.Threading.Tasks;
using Models.CustomModels;
using Models.ServiceModels.Dashboard;
using Services.Contracts;
using Services.Managers.Contracts;

namespace Web.Pages
{
    public class IndexModel : BasePageModel
    {
        #region Private Fields

        private readonly IDashboardService _dashboardService;
        private readonly ISessionManager _sessionManager;


        #endregion

        #region Properties
        public int TotalRunningBudget { get; private set; }
        public int TotalCompletedBudget { get; private set; }
        public int TotalDraftedBudget { get; set; }
        public int TotalPendingBudget { get; set; }
        public int TotalRollbackedBudget { get; set; }
        public int TotalRejectedBudget { get; set; }
        public List<DashboardTotalAmountVM> TotalAmountWithCurrency { get;  set; }
        public List<SimpleReportViewModel> PieReportViewModel { get;  set; }
        public List<SimpleReportViewModel> BarReportViewModel { get;  set; }
        public StackedViewModel StackedViewModel { get;  set; }

        #endregion

        #region Constructors

        public IndexModel(IDashboardService dashboardService, ISessionManager sessionManager)
        {
            _dashboardService = dashboardService;
            _sessionManager = sessionManager;
        }

        #endregion

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
                var pieData = new List<SimpleReportViewModel>
            {
                new SimpleReportViewModel
                {
                    DimensionOne = "Running",
                    Quantity = 9
                },
                new SimpleReportViewModel
                {
                    DimensionOne = "Completed",
                    Quantity = responseForNav.TotalCompletedParking
                },
                new SimpleReportViewModel
                {
                    DimensionOne = "Draft",
                    Quantity = responseForNav.TotalDraftParking
                },
                new SimpleReportViewModel
                {
                    DimensionOne = "Pending",
                    Quantity = responseForNav.TotalPendingApprovalParking
                },
                new SimpleReportViewModel
                {
                    DimensionOne = "RollBack",
                    Quantity = responseForNav.TotalRollbackParking
                },
                new SimpleReportViewModel
                {
                    DimensionOne = "Rejected",
                    Quantity = 8
                }
            };
                if (response.IsSuccessful)
                {
                    TotalRunningBudget = 8;
                    TotalCompletedBudget = responseForNav.TotalCompletedParking;
                    TotalDraftedBudget = responseForNav.TotalDraftParking;
                    TotalAmountWithCurrency = new List<DashboardTotalAmountVM>();
                    TotalPendingBudget = responseForNav.TotalPendingApprovalParking;
                    TotalRollbackedBudget = responseForNav.TotalRollbackParking;
                    TotalRejectedBudget = 6;
                    PieReportViewModel = pieData;
                }

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

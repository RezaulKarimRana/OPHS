using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AMS.Models.CustomModels;
using AMS.Models.ServiceModels.BudgetEstimate;
using AMS.Models.ServiceModels.Dashboard;
using AMS.Services.Budget.Contracts;
using AMS.Services.Contracts;
using AMS.Services.Managers.Contracts;

namespace AMS.Web.Pages
{
    public class IndexModel : BasePageModel
    {
        #region Private Fields

        private readonly IDashboardService _dashboardService;
        private readonly IBudgetService _budgetService;
        private readonly ISessionManager _sessionManager;


        #endregion

        #region Properties

        //public int TotalSessions { get; set; }

        //public int TotalUsers { get; set; }

        //public int TotalRoles { get; set; }

        //public int TotalConfigItems { get; set; }
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

        public IndexModel(IDashboardService dashboardService, IBudgetService budgetService, ISessionManager sessionManager)
        {
            _dashboardService = dashboardService;
            _budgetService  = budgetService;
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
                List<EstimateEditVM> runningBudgetList = await _budgetService.LoadAllPendingEstimate(sessionUser.Id, 0, 0);
                List<EstimateEditVM> rejectedBUdgetList = await _budgetService.LoadRejectedEstimate(sessionUser.Id, 0, 0);
                var response = await _dashboardService.GetIndexDashBoard();
                var totalBudgetAmoutSumByUserWithCurrency = await _budgetService.GetAllBudgetAmountSumByUserId();
                var responseForNav = await _dashboardService.GetNavBarCount();
                var pieData = new List<SimpleReportViewModel>
            {
                new SimpleReportViewModel
                {
                    DimensionOne = "Running",
                    Quantity = runningBudgetList.Count
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
                    Quantity = rejectedBUdgetList.Count
                }
            };
                if (response.IsSuccessful)
                {
                    TotalRunningBudget = runningBudgetList.Count;
                    TotalCompletedBudget = responseForNav.TotalCompletedParking;
                    TotalDraftedBudget = responseForNav.TotalDraftParking;
                    TotalAmountWithCurrency = totalBudgetAmoutSumByUserWithCurrency;
                    TotalPendingBudget = responseForNav.TotalPendingApprovalParking;
                    TotalRollbackedBudget = responseForNav.TotalRollbackParking;
                    TotalRejectedBudget = rejectedBUdgetList.Count;
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

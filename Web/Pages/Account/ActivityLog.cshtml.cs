using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using AMS.Models.DomainModels;
using AMS.Models.ServiceModels.Account;
using AMS.Services.Contracts;
using AMS.Models.ServiceModels.Dashboard;

namespace AMS.Web.Pages
{
    public class ActivityLogModel : BasePageModel
    {
        #region Private Fields

        private readonly IAccountService _accountService;
        private readonly IDashboardService _dashboardService;

        #endregion

        #region Properties

        public UserEntity? UserEntity { get; set; }

        public List<ActivityLog> ActivityLogs { get; set; }

        #endregion

        #region Constructors

        public ActivityLogModel(IAccountService accountService, IDashboardService dashboardService)
        {
            _accountService = accountService;
            _dashboardService = dashboardService;
            ActivityLogs = new List<ActivityLog>();
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
            var response = await _accountService.GetProfile();
            UserEntity = response.User;
        }

        public async Task<JsonResult> OnGetData()
        {
            var response = await _accountService.GetActivityLogs(new GetActivityLogsRequest());
            return new JsonResult(response);
        }
    }
}

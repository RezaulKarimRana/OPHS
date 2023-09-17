using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AMS.Models.ServiceModels.BudgetEstimate;
using AMS.Models.ServiceModels.Dashboard;
using AMS.Repositories.UnitOfWork.Contracts;
using AMS.Services.Budget.Contracts;
using AMS.Services.Contracts;
using AMS.Services.Managers.Contracts;

namespace AMS.Services
{
    public class DashboardService : IDashboardService
    {
        #region Instance Fields

        private readonly IUnitOfWorkFactory _uowFactory;
        private readonly ISessionManager _sessionManager;
        private readonly IBudgetService _budgetService;

        #endregion

        #region Constructor

        public DashboardService(IUnitOfWorkFactory uowFactory, ISessionManager sessionManager, IBudgetService budgetService)
        {
            _uowFactory = uowFactory;
            _sessionManager = sessionManager;
            _budgetService = budgetService;
        }

        #endregion

        #region Public Methods

        public async Task<GetIndexDashBoardResponse> GetIndexDashBoard()
        {
            var response = new GetIndexDashBoardResponse();

            using (var uow = _uowFactory.GetUnitOfWork())
            {
                var dashboardResponse = await uow.DashboardRepo.GetDashboard();

                response = new GetIndexDashBoardResponse()
                {
                    TotalConfigItems = dashboardResponse.TotalConfigItems,
                    TotalRoles = dashboardResponse.TotalRoles,
                    TotalSessions = dashboardResponse.TotalSessions,
                    TotalUsers = dashboardResponse.TotalUsers,
                    TotalRunningBudget = dashboardResponse.TotalRunningBudget,

                    TotalCompletedBudget = dashboardResponse.TotalCompletedBudget,
                    TotalDraftedBudget = dashboardResponse.TotalDraftedBudget,
                    TotalAmount = dashboardResponse.TotalAmount
            };
            }

            return response;
        }

        public async Task<GetCountForAllPendingParkingForNavService> GetNavBarCount()
        {
            try
            {
                var sessionUser = await _sessionManager.GetUser();
                if (sessionUser == null)
                {
                    throw new Exception("user session expired");
                }

                using var uow = _uowFactory.GetUnitOfWork();

                var navCount = await uow.DashboardRepo.GetNavBarCount(sessionUser.Id);

                var result = await _budgetService.LoadAllPendingEstimateByUser(sessionUser.Id);
                var data = new List<object>();
                var resultSet = new List<EstimateVM>();
                foreach (var item in result)
                {
                    bool isValid = await _budgetService.IsValidToShowInParking(item.EstimationId, item.Priority);
                    if (!isValid) continue;
                    resultSet.Add(item);
                }

                var recordsTotal = resultSet.Count;

                var response = new GetCountForAllPendingParkingForNavService()
                {
                    TotalCompletedParking = navCount.TotalCompletedParking,
                    TotalDraftParking = navCount.TotalDraftParking,
                    //TotalPendingApprovalParking = navCount.TotalPendingApprovalParking,
                    TotalRollbackParking = navCount.TotalRollbackParking
                };
                response.TotalPendingApprovalParking = recordsTotal;
                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion
    }
}

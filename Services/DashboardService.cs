﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Models.ServiceModels.Dashboard;
using Repositories.UnitOfWork.Contracts;
using Services.Contracts;
using Services.Managers.Contracts;

namespace Services
{
    public class DashboardService : IDashboardService
    {
        #region Instance Fields

        private readonly IUnitOfWorkFactory _uowFactory;
        private readonly ISessionManager _sessionManager;

        #endregion

        #region Constructor

        public DashboardService(IUnitOfWorkFactory uowFactory, ISessionManager sessionManager)
        {
            _uowFactory = uowFactory;
            _sessionManager = sessionManager;
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
                var data = new List<object>();

                var recordsTotal = 0;

                var response = new GetCountForAllPendingParkingForNavService()
                {
                    TotalCompletedParking = navCount.TotalCompletedParking,
                    TotalDraftParking = navCount.TotalDraftParking,
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

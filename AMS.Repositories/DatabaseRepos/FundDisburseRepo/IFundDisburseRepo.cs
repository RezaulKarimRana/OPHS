using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AMS.Models.CustomModels;
using AMS.Models.ServiceModels.FundDisburse;

namespace AMS.Repositories.DatabaseRepos.FundDisburseRepo
{
    public interface IFundDisburseRepo
    {
        Task<List<FundDisburseHistory>> GetFundDisburseHistoryByEstimateId(int estimationId);
        Task<int> CreateFundDisburse(Models.DomainModels.FundDisburse request);
        Task<int> FundDisburseReceiveOrRollback(Models.DomainModels.FundDisburse request);

        Task<List<FundDisburseVM>> PendingFundDisburseListForAcknowledgement(int userId, int currentPageIndex,
            int pAGE_SIZE, int UserId, int departmentId);

        Task<List<FundDisburseVM>> PendingRollBackListForReSubmitByFinance(int userId, int currentPageIndex,
            int pAGE_SIZE, int UserId, int departmentId);

        Task<List<FundDisburseVM>> CompletedDisburseListDepartmentWise(int userId, int currentPageIndex,
            int pAGE_SIZE, int UserId, int departmentId);

        Task<FundDisburseVM> GetFundDisburseHistoryByFundDisburseId(int fundDisburseId);

        Task<int> FundReDisburse(Models.DomainModels.FundDisburse request);

        Task<List<FundDisburseVM>> FundDisburseByFinanceWaitingForAcknowledge(int userId, int currentPageIndex,
            int pAGE_SIZE, int UserId, int departmentId);


    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AMS.Models.CustomModels;
using AMS.Models.DomainModels;
using AMS.Models.ServiceModels.FundDisburse;
using AMS.Models.ServiceModels.FundRequisition;
using AMS.Repositories.UnitOfWork.Contracts;

namespace AMS.Services.FundDisburseService
{
    public interface IFundDisburseService
    {
        Task<List<FundDisburseHistory>> GetFundDisburseHistoryByEstimateId(int estimationId);
        Task<int> addFundDisburse(FundDisburse requestDto);
        Task<CreateFundDisburseResponse> CreateFundDisburse(FundDisburse fundDisburseRequest, IUnitOfWork uow);

        Task<List<FundDisburseVM>> PendingFundDisburseListForAcknowledgement(int userId, int currentPageIndex,
            int pAGE_SIZE);

        Task<List<FundDisburseVM>> PendingRollBackListForReSubmitByFinance(int userId, int currentPageIndex,
            int pAGE_SIZE);

        Task<List<FundDisburseVM>> CompletedDisburseListDepartmentWise(int userId, int currentPageIndex, int pAGE_SIZE);
        Task<FundDisburseVM> GetFundDisburseHistoryByFundDisburseId(int fundDisburseId);
        Task<int> FundDisburseReceiveOrRollback(FundDisburse requestDto);

        Task<CreateFundDisburseResponse> CreateFundDisburseReceiveOrRollback(FundDisburse fundDisburseRequest,
            IUnitOfWork uow);

        Task<int> FundReDisburse(FundDisburse requestDto);
        Task<CreateFundDisburseResponse> CreateFundReDisburse(FundDisburse fundDisburseRequest, IUnitOfWork uow);

        Task<List<FundDisburseVM>> FundDisburseByFinanceWaitingForAcknowledge(int userId, int currentPageIndex,
            int pAGE_SIZE);




    }
}

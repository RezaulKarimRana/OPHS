using AMS.Models.DomainModels;
using AMS.Models.ServiceModels.FundRequisition;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AMS.Models.CustomModels;
using AMS.Models.ServiceModels.Settlement;
using AMS.Repositories.UnitOfWork.Contracts;

namespace AMS.Services.FundRequisitionService
{
    public interface IFundRequisitionService
    {
        Task<int> addFundRequisition(FundRequisition requestDto);
        Task<List<FundRequisitionVM>> SubmitedFundRequistionList(int userId, int currentPageIndex, int pAGE_SIZE);
        Task<List<FundRequisitionVM>> FundRequistionListForFinance(int userId, int currentPageIndex, int pAGE_SIZE);
        Task<FundRequisitionVM> GetFundRequisitionHistoryByFundRequisitionId(int fundRequisitioId);
        Task<FundRequisitionVM> GetFundRequisitionHistoryByEstimationId(int estimationId);
        Task<List<FundRequisitionDisburseHistory>> getFundRequisitionDisburseHistory(int estimationId);
        Task<int> UpdateFundRequistionStatus(int fundRequistionId, string Remarks);

        Task<List<FundRequisitionVM>> FundRequistionListByStatus(int userId, int currentPageIndex, int pAGE_SIZE,
            int status);

 
       
        Task<List<FundRequisitionDisburseHistory>> getFundRequisitionDisburseHistoryOfOtherDepartment(int estimationId);

        Task<List<FundRequisitionVM>> getInCompletedFundRequistionForCheckFinalSettlement(int estimationId);
        Task<List<FundRequisitionDisburseHistory>> getTotalFundRequisitionDisburseHistory(int estimationId);

        Task<List<FundRequisitionVM>> RejectedFundRequistionListForFinance(int userId, int currentPageIndex,
            int pAGE_SIZE);
    }
}

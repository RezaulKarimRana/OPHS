using AMS.Models.ServiceModels.FundRequisition;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AMS.Models.CustomModels;


namespace AMS.Repositories.DatabaseRepos.FundRequisition
{
    public interface IFundRequisitionRepo
    {
        Task<int> CreateFundRequisition(Models.DomainModels.FundRequisition request);
        Task<List<FundRequisitionVM>> SubmitedFundRequistionList(int userId, int currentPageIndex, int pAGE_SIZE, int UserId, int departmentId);
        Task<List<FundRequisitionVM>> FundRequistionListForFinance(int userId, int currentPageIndex, int pAGE_SIZE, int UserId, int departmentId);
        Task<FundRequisitionVM> GetFundRequisitionHistoryByFundRequisitionId(int fundRequisitionId);
        Task<FundRequisitionVM> GetFundRequisitionHistoryByEstimationId(int estimationId , int userId , int departmentId);

        Task<List<FundRequisitionDisburseHistory>> getFundRequisitionDisburseHistory(int userId, int departmentId,
            int estimationId);

        Task<int> UpdateFundRequistionStatus(int fundRequistionId, string remarks, int feedback, int userId);

        Task<List<FundRequisitionVM>> FundRequistionListByStatus(int userId, int currentPageIndex, int pAGE_SIZE,
            int UserId, int departmentId, int status);

        Task<string> getUserEmailAddressByDepartmentId(int departmentId);

        Task<List<FundRequisitionDisburseHistory>> getFundRequisitionDisburseHistory_of_other_department(int userId,
            int departmentId,
            int estimationId);

        Task<List<FundRequisitionVM>> getInCompletedFundRequisitionForCheckFinalSettlement(int userId, int departmentId,
            int estimationId);

        Task<List<FundRequisitionDisburseHistory>> getTotalFundRequisitionDisburseHistory(int userId, int departmentId,
            int estimationId);

        Task<List<FundRequisitionVM>> RejectedFundRequistionListForFinance(int userId, int currentPageIndex,
            int pAGE_SIZE, int UserId, int departmentId);
    }
}

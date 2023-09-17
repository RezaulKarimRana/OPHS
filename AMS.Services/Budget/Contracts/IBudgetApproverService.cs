using AMS.Models.CustomModels;
using AMS.Models.DomainModels;
using AMS.Models.ServiceModels.BudgetEstimate.BudgetApprover;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AMS.Services.Budget.Contracts
{
    public interface IBudgetApproverService
    {
        Task<EstimateApproverEntity> GetApproverByEstimateANDUser(GetApproverByEstimateAndUser request);
        Task<List<EstimateApproverEntity>> GetLatestPendingApprovers(int estimationId);
        Task<UpdateApproverStatusResponse> UpdateEstimateApproverStatusById(int id, string status, string remarks);
        Task<int> CreateApproverFeedBack(CreateApproverFeedBackServiceRequest request);
        Task<EstimateApproverFeedbackEntity> GetFeedbackByEstimationandUserId(int estimate_Id, int userId, int completed);
        Task DeleteAllApproversByEstimateId(int EstimateId);
        Task<List<LoadApproverFeedBackDetails>> LoadApproverRemarksService(int estimationId);
        Task<int> GetUpcomingPendingApproverLevel(int estimateId);
        Task<UpdateApproverStatusResponse> FinalApproverConvertToInformer(int estimateId);
    }
}

using AMS.Models.CustomModels;
using AMS.Models.DomainModels;
using AMS.Repositories.DatabaseRepos.EstimateApproverRepo.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AMS.Repositories.DatabaseRepos.EstimateApproverRepo.Contracts
{
    public interface IEstimateApproverRepo
    {
        Task<int> CreateEstimateApprover(CreateEstimateApproverRequest request);
        Task<List<EstimateApproverEntity>> GetAllEstimateApproveres();
        Task<List<EstimateApproverEntity>> GetLatestPendingApproveresOfAEstimation(int estimationId);
        Task<EstimateApproverEntity> GetSingleEstimateApprover(int id);
        Task UpdateEstimateApprover(UpdateEstimateApproverRequest request);
        Task DeleteEstimateApprover(DeleteEstimateApproverRequest request);
        Task<List<EstimateApproverEntity>> LoadEstimateApproverByEstimationId(int estimationId);
        Task<List<EstimateApproverByEstimateId>> LoadEstimateApproverDetailsByEstimationId(int estimationId);
        Task CompleteApproverFeedback(int estimateId, int userId, string feedback, string remarks);
        Task DeleteApproverByEstimate(int estimationId);
        Task<EstimateApproverEntity> GetApproverByEstimateIdAndUserId(int estimateId, int userId);
        Task UpdateApproverStatus(UpdateApproverStatusRequest request);
        Task<int> GetLatestPendingApproverLevel(int estimateId);
        Task FinalApproverConvertToInformer(int estimateId);
    }
}

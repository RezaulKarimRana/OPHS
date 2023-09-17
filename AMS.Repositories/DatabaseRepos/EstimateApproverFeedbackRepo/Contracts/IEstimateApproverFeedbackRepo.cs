using AMS.Models.CustomModels;
using AMS.Models.DomainModels;
using AMS.Repositories.DatabaseRepos.EstimateApproverFeedbackRepo.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AMS.Repositories.DatabaseRepos.EstimateApproverFeedbackRepo.Contracts
{
    public interface IEstimateApproverFeedbackRepo
    {
        Task<int> CreateEstimateApproverFeedback(CreateApproverFeedbackRequest request);
        Task<EstimateApproverFeedbackEntity?> GetFeedbackByEstimationandUserId(int estimate_Id, int userId, int completed);
        Task<List<LoadApproverFeedBackDetails>> LoadApproverFeedBackDetails(int estimationId);
    }
}

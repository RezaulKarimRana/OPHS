using AMS.Models.CustomModels;
using AMS.Models.DomainModels;
using AMS.Repositories.DatabaseRepos.EstimateDetailsRepo.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AMS.Repositories.DatabaseRepos.EstimateDetailsRepo.Contracts
{
    public interface IEstimateDetailsRepo
    {
        Task<int> CreateEstimateDetails(CreateEstimateDetailsRequest request);
        Task<List<EstimateDetailsEntity>> GetAllEstimateDetails();
        Task<EstimateDetailsEntity> GetEstimateDetails(int id);
        Task UpdateEstimateDetails(UpdateEstimateDetailsRequest request);
        Task DeleteEstimateDetails(DeleteEstimateDetailsRequest request);
        Task<List<EstimateDetailsEntity>> LoadEstimateDetailByEstimationId(int estimationId);
        Task<List<EstimationDetailsWithJoiningOtherTables>> LoadEstimationDetailsWithOtherInformationsByEstimationId(int estimationId);
        Task DeleteEstimateDetailsByEstimate(int estimateId);
    }
}

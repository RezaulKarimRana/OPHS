using AMS.Models.DomainModels;
using AMS.Repositories.DatabaseRepos.EstimateChangeHistoryRepo.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AMS.Repositories.DatabaseRepos.EstimateChangeHistoryRepo.Contracts
{
    public interface IEstimateChangeHistoryRepo
    {
        Task<int> CreateEstimationHistory(CreateEstimationHistoryRequest request);
        Task<List<EstimateChangeHistoryEntity>> GetAllEstimationHistory();
        Task<EstimateChangeHistoryEntity> GetSingleEstimationHistory(int id);
    }
}

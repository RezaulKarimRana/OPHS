using AMS.Models.DomainModels;
using AMS.Repositories.DatabaseRepos.EstimateDetailsChangeHistoryRepo.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AMS.Repositories.DatabaseRepos.EstimateDetailsChangeHistoryRepo.Contracts
{
    public interface IEstimateDetailsChangeHistoryRepo
    {
        Task<int> CreateEstimateDetailsHistory(CreateEstimateDetailsChangeHistoryRequest request);
        Task<List<EstimateDetailsChangeHistoryEntity>> GetAllEstimateDetails();
        Task<EstimateDetailsChangeHistoryEntity> GetEstimateDetails(int id);
    }
}

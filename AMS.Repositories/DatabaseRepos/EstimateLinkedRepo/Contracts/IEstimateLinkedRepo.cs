using AMS.Models.DomainModels;
using AMS.Repositories.DatabaseRepos.EstimateLinkedRepo.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AMS.Repositories.DatabaseRepos.EstimateLinkedRepo.Contracts
{
    public interface IEstimateLinkedRepo
    {
        Task<int> CreateEstimateLinked(CreateEstimateLinkedRequest request);
        Task<List<EstimateLinkedEntity>> GetAllEstimateLinked();
        Task<EstimateLinkedEntity> GetSingleEstimateLinked(int id);
    }
}

using AMS.Models.DomainModels;
using AMS.Repositories.DatabaseRepos.ParticularWiseSummaryHistoryRepo.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AMS.Repositories.DatabaseRepos.ParticularWiseSummaryHistoryRepo.Contracts
{
    public interface IParticularWiseSummaryHistoryRepo
    {
        Task<int> CreateParticularWiseSummaryHistory(CreateParticularWiseSummaryHistoryRequest request);
        Task<List<ParticularWiseSummaryHistoryEntity>> GetAllParticularWiseSummaryHistory();
        Task<ParticularWiseSummaryHistoryEntity> GetSingleParticularWiseSummaryHistory(int id);
    }
}

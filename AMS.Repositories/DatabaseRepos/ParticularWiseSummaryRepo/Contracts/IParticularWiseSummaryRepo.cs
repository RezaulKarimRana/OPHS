using AMS.Models.CustomModels;
using AMS.Models.DomainModels;
using AMS.Repositories.DatabaseRepos.ParticularWiseSummaryRepo.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AMS.Repositories.DatabaseRepos.ParticularWiseSummaryRepo.Contracts
{
    public interface IParticularWiseSummaryRepo
    {
        Task<int> CreateParticularWiseSummary(CreateParticularWiseSummaryRequest request);
        Task<List<ParticularWiseSummaryEntity>> GetAllParticularWiseSummary();
        Task<ParticularWiseSummaryEntity> GetSingleParticularWiseSummary(int id);
        Task<List<ParticularWiseSummaryEntity>> LoadParticularWiseSummaryByEstimationId(int estimationId);
        Task<List<ParticularWiseSummaryDetailsByEstimationId>> LoadParticularWiseSummaryDetailsByEstimationId(int estimateId);
        Task DeleteParticularSummaryByEstimateId(int estimateId);
    }
}

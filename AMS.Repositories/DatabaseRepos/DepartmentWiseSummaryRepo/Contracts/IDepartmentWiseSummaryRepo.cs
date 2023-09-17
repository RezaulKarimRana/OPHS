using AMS.Models.CustomModels;
using AMS.Models.DomainModels;
using AMS.Repositories.DatabaseRepos.DepartmentWiseSummaryRepo.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AMS.Repositories.DatabaseRepos.DepartmentWiseSummaryRepo.Contracts
{
    public interface IDepartmentWiseSummaryRepo
    {
        Task<int> CreateDepartmentWiseSummary(CreateDepartmentWiseSummaryRequest request);
        Task<List<DepartmentWiseSummaryEntity>> GetAllDepartmentWiseSummary();
        Task<DepartmentWiseSummaryEntity> GetSingleDepartmentWiseSummary(int id);
        Task<List<DepartmentWiseSummaryEntity>> LoadDepartmentWiseSummaryByEstimationId(int estimationId);
        Task<List<DepartWiseSummaryDetailsByEstimationId>> LoadDepartmentWiseSummaryDetailsByEstimationId(int estimationId);
        Task DeleteDepartmentSummaryByEstimate(int estimateId);
    }
}

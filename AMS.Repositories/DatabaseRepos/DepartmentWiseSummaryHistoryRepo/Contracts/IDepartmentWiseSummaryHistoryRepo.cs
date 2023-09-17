using AMS.Models.DomainModels;
using AMS.Repositories.DatabaseRepos.DepartmentWiseSummaryHistoryRepo.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AMS.Repositories.DatabaseRepos.DepartmentWiseSummaryHistoryRepo.Contracts
{
    public interface IDepartmentWiseSummaryHistoryRepo
    {
        Task<int> CreateDepartmentWiseSummaryHistory(CreateDepartmentWiseSummaryHistoryRequest request);
        Task<List<DepartmentWiseSummaryHistoryEntity>> GetAllDepartmentWiseSummaryHistory();
        Task<DepartmentWiseSummaryHistoryEntity> GetSingleDepartmentWiseSummaryHistory(int id);
    }
}

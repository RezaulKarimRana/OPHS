using AMS.Models.DomainModels;
using AMS.Repositories.DatabaseRepos.DivRepo.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AMS.Repositories.DatabaseRepos.DivRepo.Contracts
{
    public interface IDivRepo
    {
        Task<int> CreateDiv(CreateDivRequest request);

        Task<List<DivEntity>> GetAllDiv();

        Task<DivEntity> GetSingleDiv(int id);

        Task UpdateDiv(UpdateDivRequest request);

        Task DeleteDiv(DeleteDivRequest request);
    }
}

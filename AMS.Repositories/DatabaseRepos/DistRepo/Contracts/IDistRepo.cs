using AMS.Models.DomainModels;
using AMS.Repositories.DatabaseRepos.DistRepo.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AMS.Repositories.DatabaseRepos.DistRepo.Contracts
{
    public interface IDistRepo
    {
        Task<int> CreateDist(CreateDistRequest request);
        Task<List<DistEntity>> GetAllDist();
        Task<DistEntity> GetSingleDist(int id);
        Task UpdateDist(UpdateDistRequest request);
        Task DeleteDist(DeleteDistRequest request);
    }
}

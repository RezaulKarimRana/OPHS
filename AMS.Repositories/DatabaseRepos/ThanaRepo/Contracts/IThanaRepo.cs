using AMS.Models.DomainModels;
using AMS.Repositories.DatabaseRepos.ThanaRepo.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AMS.Repositories.DatabaseRepos.ThanaRepo.Contracts
{
    public interface IThanaRepo
    {
        Task<int> CreateThana(CreateThanaRequest request);
        Task<List<ThanaEntity>> GetAllThana();
        Task<ThanaEntity> GetSingleThana(int id);
        Task UpdateThana(UpdateThanaRequest request);
        Task DeleteThana(DeleteThanaRequest request);
        Task<IList<ThanaEntity>> GetThanaByDistId(int DistId);
    }
}

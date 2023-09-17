using AMS.Models.DomainModels;
using AMS.Models.ViewModel;
using AMS.Repositories.DatabaseRepos.UnitRepo.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AMS.Repositories.DatabaseRepos.UnitRepo.Contracts
{
    public interface IUnitRepo
    {
        Task<int> CreateUnit(CreateUnitRequest request);
        Task<List<UnitEntity>> GetAllUnits();
        Task<UnitEntity> GetSingleUnit(int id);
        Task UpdateUnit(UpdateUnitRequest request);
        Task DeleteUnit(DeleteUnitRequest request);
        Task<List<NameIdPairModel>> GetAllAsNameIdPair();
    }
}

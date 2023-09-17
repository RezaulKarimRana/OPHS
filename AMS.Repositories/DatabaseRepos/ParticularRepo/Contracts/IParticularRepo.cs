using AMS.Models.DomainModels;
using AMS.Models.ViewModel;
using AMS.Repositories.DatabaseRepos.ParticularRepo.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AMS.Repositories.DatabaseRepos.ParticularRepo.Contracts
{
    public interface IParticularRepo
    {
        Task<int> CreateParticular(CreateParticularRequest request);

        Task<List<ParticularEntity>> GetAllParticular();

        Task<ParticularEntity> GetSingleParticular(int id);

        Task UpdateParticular(UpdateParticularRequest request);

        Task DeleteParticular(DeleteParticularRequest request);
        Task<List<NameIdPairModel>> GetAllAsNameIdPair();
    }
}

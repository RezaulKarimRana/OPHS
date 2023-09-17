using AMS.Models.DomainModels;
using AMS.Models.ServiceModels.Admin.Particular;
using System.Threading.Tasks;

namespace AMS.Services.Admin.Contracts
{
    public interface IParticularService
    {
        Task<GetParticularsResponse> GetParticulars();
        Task<ParticularEntity> GetById(int id);
    }
}

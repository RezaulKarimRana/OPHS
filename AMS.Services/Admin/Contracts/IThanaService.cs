using AMS.Models.DomainModels;
using AMS.Models.ServiceModels.Admin.Thana;
using System.Threading.Tasks;

namespace AMS.Services.Admin.Contracts
{
    public interface IThanaService
    {
        Task<GetThanaByDistIDResponse> getThanaByDistIDResponse(int distId);
        Task<ThanaEntity> GetById(int thana_Id);
    }
}

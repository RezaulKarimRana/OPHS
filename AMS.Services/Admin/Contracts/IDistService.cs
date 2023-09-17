using AMS.Models.DomainModels;
using AMS.Models.ServiceModels.Admin.Dist;
using System.Threading.Tasks;

namespace AMS.Services.Admin.Contracts
{
    public interface IDistService
    {
        Task<GetAllDistrictResponse> GetAllDistrict();
        Task<DistEntity> GetById(int district_Id);
    }
}

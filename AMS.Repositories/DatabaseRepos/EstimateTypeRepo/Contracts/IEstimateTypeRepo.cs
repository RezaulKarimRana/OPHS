using AMS.Models.DomainModels;
using AMS.Models.ServiceModels.Admin.Dropdown;
using AMS.Repositories.DatabaseRepos.EstimateTypeRepo.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AMS.Repositories.DatabaseRepos.EstimateTypeRepo.Contracts
{
    public interface IEstimateTypeRepo
    {
        Task<int> CreateEstimateType(CreateEstimateTypeRequest request);
        Task<List<EstimateTypeEntity>> GetAllEstimateType();
        Task<EstimateTypeEntity> GetSingleEstimateType(int id);
        Task UpdateEstimateType(UpdateEstimateTypeRequest request);
        Task DeleteEstimateType(DeleteEstimateTypeRequest request);

        Task<List<EstimationDropdown>> LoadAllDropdownItemByDropdownNameAndEstimationType(string dropDownName,
            int estimationType);
    }
}

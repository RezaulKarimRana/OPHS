using AMS.Models.ServiceModels.Admin.Dropdown;
using AMS.Models.ServiceModels.Admin.EstimateType;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AMS.Services.Admin.Contracts
{
    public interface IEstimateTypeService
    {
        Task<LoadAllEstimateType> LoadAllTheEstimateTypes();

        Task<List<EstimationDropdown>> LoadAllDropdownItemByDropdownNameAndEstimationType(string dropDownName,
            int estimationType);
    }
}

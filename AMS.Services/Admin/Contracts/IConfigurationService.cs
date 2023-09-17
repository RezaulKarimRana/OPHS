using System.Threading.Tasks;
using AMS.Models.ServiceModels.Admin.Configuration;

namespace AMS.Services.Admin.Contracts
{
    public interface IConfigurationService
    {
        Task<GetConfigurationItemsResponse> GetConfigurationItems();

        Task<GetConfigurationItemResponse> GetConfigurationItem(GetConfigurationItemRequest request);

        Task<UpdateConfigurationItemResponse> UpdateConfigurationItem(UpdateConfigurationItemRequest request);

        Task<CreateConfigurationItemResponse> CreateConfigurationItem(CreateConfigurationItemRequest request);
    }
}

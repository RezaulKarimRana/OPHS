using System.Collections.Generic;
using System.Threading.Tasks;
using AMS.Repositories.DatabaseRepos.ConfigurationRepo.Models;
using Models.DomainModels;

namespace AMS.Repositories.DatabaseRepos.ConfigurationRepo.Contracts
{
    public interface IConfigurationRepo
    {
        Task<List<ConfigurationEntity>> GetConfigurationItems();

        Task UpdateConfigurationItem(UpdateConfigurationItemRequest request);

        Task<int> CreateConfigurationItem(CreateConfigurationItemRequest request);
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using Repositories.DatabaseRepos.ConfigurationRepo.Models;
using Models.DomainModels;

namespace Repositories.DatabaseRepos.ConfigurationRepo.Contracts
{
    public interface IConfigurationRepo
    {
        Task<List<ConfigurationEntity>> GetConfigurationItems();

        Task UpdateConfigurationItem(UpdateConfigurationItemRequest request);

        Task<int> CreateConfigurationItem(CreateConfigurationItemRequest request);
    }
}

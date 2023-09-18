using System.Collections.Generic;
using Models.DomainModels;

namespace Models.ServiceModels.Admin.Configuration
{
    public class GetConfigurationItemsResponse
    {
        public List<ConfigurationEntity> ConfigurationItems { get; set; }

        public GetConfigurationItemsResponse()
        {
            ConfigurationItems = new List<ConfigurationEntity>();
        }
    }
}

using System.Collections.Generic;
using AMS.Models.DomainModels;

namespace AMS.Models.ServiceModels.Admin.Configuration
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

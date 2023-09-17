using AMS.Models.DomainModels;

namespace AMS.Models.ServiceModels.Admin.Configuration
{
    public class GetConfigurationItemResponse : ServiceResponse
    {
        public ConfigurationEntity ConfigurationItem { get; set; }
    }
}

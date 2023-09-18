using Models.DomainModels;

namespace Models.ServiceModels.Admin.Configuration
{
    public class GetConfigurationItemResponse : ServiceResponse
    {
        public ConfigurationEntity ConfigurationItem { get; set; }
    }
}

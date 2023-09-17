using AMS.Repositories.DatabaseRepos.Common;

namespace AMS.Repositories.DatabaseRepos.EstimateTypeRepo.Models
{
    public class CreateEstimateTypeRequest : CreatedBy
    {
        public string Name { get; set; }
        public string ApplicationName { get; set; }
    }
}

using AMS.Repositories.DatabaseRepos.Common;

namespace AMS.Repositories.DatabaseRepos.EstimateTypeRepo.Models
{
    public class UpdateEstimateTypeRequest : UpdatedBy
    {
        public string Name { get; set; }
        public string ApplicationName { get; set; }
    }
}

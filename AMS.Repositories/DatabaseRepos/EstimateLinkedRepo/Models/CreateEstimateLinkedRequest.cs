using AMS.Repositories.DatabaseRepos.Common;

namespace AMS.Repositories.DatabaseRepos.EstimateLinkedRepo.Models
{
    public class CreateEstimateLinkedRequest : CreatedBy
    {
        public int EstimationOldId { get; set; }
        public int EstimationNewId { get; set; }
    }
}

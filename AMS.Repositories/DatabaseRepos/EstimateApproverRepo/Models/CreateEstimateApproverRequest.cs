using AMS.Repositories.DatabaseRepos.Common;

namespace AMS.Repositories.DatabaseRepos.EstimateApproverRepo.Models
{
    public class CreateEstimateApproverRequest : CreatedBy
    {
        public int Estimate_Id { get; set; }
        public int User_Id { get; set; }
        public int Priority { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
        public int RolePriority_Id { get; set; }
        public string PlanDate { get; set; }

    }
}

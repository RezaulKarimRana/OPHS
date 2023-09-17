using AMS.Repositories.DatabaseRepos.Common;

namespace AMS.Repositories.DatabaseRepos.EstimateApproverChangeHistoryRepo.Models
{
    public class CreateEstimateApproverChangeHistoryRequest : CreatedBy
    {
        public int Estimate_Id { get; set; }
        public int User_Id { get; set; }
        public int Priority { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
        public int RolePriority_Id { get; set; }
        public string PlanDate { get; set; }
        //public int EstimateApprover_Id { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace AMS.Repositories.DatabaseRepos.ProcurementApproval.Models
{
    public class EstitmationApprovalInfo
    {
        public int EstimationId { get; set; }
        public string UniqueIdentifier { get; set; }
        public double TotalPrice { get; set; }
        public int EstimationStatus { get; set; }
        public int EstimateType_Id { get; set; }
        public int RolePriority_Id { get; set; }
        public int ApprovalStatus { get; set; }
        public int User_Id { get; set; }
        public int Priority { get; set; }
        public DateTime PlanDate { get; set; }
        public string ViewURL { get; set; }
    }
}

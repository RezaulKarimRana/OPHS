using System;

namespace Models.DomainModels
{
    public class EstimateApproverEntity : BaseEntity
    {
        public int Estimate_Id { get; set; }
        public int User_Id { get; set; }
        public int Priority { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
        public DateTime PlanDate { get; set; }
        public int RolePriority_Id { get; set; }
    }
}

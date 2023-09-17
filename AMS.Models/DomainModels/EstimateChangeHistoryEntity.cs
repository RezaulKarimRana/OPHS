using System;

namespace AMS.Models.DomainModels
{
    public class EstimateChangeHistoryEntity : BaseEntity
    {
        public string EstimateType { get; set; }
        public string Status { get; set; }
        public bool IsCheckerCompleted { get; set; }
        public bool IsVarifierCompleted { get; set; }
        public bool IsRecommmenderCompleted { get; set; }
        public bool IsFinalApprovalCompleted { get; set; }
        public bool IsCheckerAssigned { get; set; }
        public bool IsVerifierAssigned { get; set; }
        public bool IsRecommenderAssigned { get; set; }
        public bool IsFinalApproverAssigned { get; set; }
        public string SystemID { get; set; }
        public int ProjectId { get; set; }
        public string Subject { get; set; }
        public string Objective { get; set; }
        public string Details { get; set; }
        public DateTime PlanStartDate { get; set; }
        public DateTime PlanEndDate { get; set; }
        public string Remarks { get; set; }
        public Double TotalPrice { get; set; }
    }
}

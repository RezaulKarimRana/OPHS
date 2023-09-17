using AMS.Repositories.DatabaseRepos.Common;
using System;

namespace AMS.Repositories.DatabaseRepos.EstimateChangeHistoryRepo.Models
{
    public class CreateEstimationHistoryRequest : CreatedBy
    {
        public int EstimateType_Id { get; set; }
        public string Status { get; set; }
        public string SystemID { get; set; }
        public int ProjectId { get; set; }
        public string Subject { get; set; }
        public string Objective { get; set; }
        public string Details { get; set; }
        public string PlanStartDate { get; set; }
        public string PlanEndDate { get; set; }
        public string Remarks { get; set; }
        public Double TotalPrice { get; set; }
        public string TotalPriceRemarks { get; set; }
        public int Estimate_Id { get; set; }
    }
}

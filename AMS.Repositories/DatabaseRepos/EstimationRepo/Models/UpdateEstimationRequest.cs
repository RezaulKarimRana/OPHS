using System;

namespace AMS.Repositories.DatabaseRepos.EstimationRepo.Models
{
    public class UpdateEstimationRequest
    {
        public int estimationId { get; set; }
        public int EstimateType { get; set; }
        public int CurrencyType { get; set; }
        public string Status { get; set; }
        public string Subject { get; set; }
        public string Objective { get; set; }
        public string Details { get; set; }
        public DateTime PlanStart { get; set; }
        public DateTime PlanEnd { get; set; }
        public double TotalPrice { get; set; }
        public string TotalPriceRemarks { get; set; }
        public int Updated_By { get; set; }
    }
}

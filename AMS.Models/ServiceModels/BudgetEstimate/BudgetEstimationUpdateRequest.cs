using System;

namespace AMS.Models.ServiceModels.BudgetEstimate
{
    public class BudgetEstimationUpdateRequest
    {
        public int EstimateId { get; set; }
        public int EstimateType { get; set; }
        public int CurrencyType { get; set; }
        public string Subject { get; set; }
        public string Objective { get; set; }
        public string Details { get; set; }
        public DateTime PlanStart { get; set; }
        public DateTime PlanEnd { get; set; }
        public double TotalPrice { get; set; }
        public string TotalPriceRemarks { get; set; }
    }
}

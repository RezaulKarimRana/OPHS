using System;

namespace AMS.Models.ServiceModels.BudgetEstimate
{
    public class BudgetEstimate
    {
        public int EstimateType { get; set; }
        public string Subject { get; set; }
        public string Objective { get; set; }
        public string Details { get; set; }
        public DateTime PlanStartDate { get; set; }
        public DateTime PlanEndDate { get; set; }
        public double TotalPrice { get; set; }
        public int Created_By { get; set; }
    }
}

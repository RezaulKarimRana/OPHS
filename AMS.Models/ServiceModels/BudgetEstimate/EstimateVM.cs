using System;

namespace AMS.Models.ServiceModels.BudgetEstimate
{
    public class EstimateVM
    {
        public int EstimationId { get; set; }
        public string EstimateType { get; set; }
        public int CurrencyType { get; set; }
        public string Status { get; set; }
        public string ProjectName { get; set; }
        public string EstimateIdentifier { get; set; }
        public string Subject { get; set; }
        public string Objective { get; set; }
        public string Details { get; set; }
        public string PlanStartDate { get; set; }
        public string PlanEndDate { get; set; }
        public string Remarks { get; set; }
        public Double TotalPrice { get; set; }
        public int Priority { get; set; }
        public string CreateorFullName { get; set; }
    }
}

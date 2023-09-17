using System;

namespace AMS.Models.CustomModels
{
    public class DraftedBudgetEstimationByUser
    {
        public int EstimationId { get; set; }
        public int EstimationTypeId { get; set; }
        public int CurrencyType { get; set; }
        public string ProjectName { get; set; }
        public string EstimationStatus { get; set; }
        public string EstimationTypeName { get; set; }
        public string EstimationSubject { get; set; }
        public string EstimationIdentity { get; set; }
        public DateTime EstimationPlanStart { get; set; }
        public DateTime EstimationPlanEnd { get; set; }
        public double EstimationTotalPrice { get; set; }
        public int TotalRow { get; set; }
        public string EstimateInitiatorWithDept { get; set; }
    }
}

using System;

namespace AMS.Models.ServiceModels.Settlement
{
    public class ReadyForSettlementVM
    {
        public int Id { get; set; }
        public string EstimationIdentity { get; set; }
        public string Subject { get; set; }
        public DateTime PlanStartDate { get; set; }
        public DateTime PlanEndDate { get; set; }
        public double TotalPrice { get; set; }
        public int TotalAllowableBudget { get; set; }
        public int TotalRequisitionAmount { get; set; }
        public int RemainingBudget { get; set; }
        public int TotalReceived { get; set; }
        public int IsItAllowableForSettlement { get; set; }
        public int DraftExists { get; set; }
        public int TotalRow { get; set; }
    }
}

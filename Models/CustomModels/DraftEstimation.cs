using System;

namespace Models.CustomModels
{
    public class DraftEstimation
    {
        public int ID { get; set; }
        public int EstimateType_Id { get; set; }
        public string Subject { get; set; }
        public string Objective { get; set; }
        public string Details { get; set; }
        public DateTime PlanStartDate { get; set; }
        public DateTime PlanEndDate { get; set; }
        public double TotalPrice { get; set; }
    }
}

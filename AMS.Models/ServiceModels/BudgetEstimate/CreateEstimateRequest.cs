using System;

namespace AMS.Models.ServiceModels.BudgetEstimate
{
    public class CreateEstimateRequest
    {
        public int Id { get; set; }
        public int EstimateType { get; set; }
        public int CurrencyType { get; set; }
        public string EstimateTypeName { get; set; }
        public string Status { get; set; }
        public string SystemID { get; set; }
        public int Project_Id { get; set; }
        public string UniqueIdentifier { get; set; }
        public string Subject { get; set; }
        public string Objective { get; set; }
        public string Details { get; set; }
        public string PlanStartDate { get; set; }
        public string PlanEndDate { get; set; }
        public string Remarks { get; set; }
        public double TotalPrice { get; set; }
        public string TotalPriceRemarks { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public DateTime CreationDate { get; set; }
      

        public CreateEstimateRequest()
        {
            SystemID = "";
            Project_Id = 0;
            Objective = "";
            Details = "";
            Remarks = "";
            TotalPrice = 0;
        }
    }
}

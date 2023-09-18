using System;

namespace Models.CustomModels
{
    public class EstimationWithEstimationType
    {
        public int EstimateId { get; set; }
        public int EstimationTypeId { get; set; }
        public int CurrencyType { get; set; }
        public string Currency { get; set; }
        public int isLineItemAvaiable { get; set; }
        public string EstimationTypeName { get; set; }
        public string EstimationTypeProject { get ; set; }
        public string EstimationStatus { get; set; }
        public string EstimationSystemID { get; set; }
        public string EstimationIdentifier { get; set; }
        public string EstimationSubject { get; set; }
        public string EstimationObjective { get; set; }
        public string EstimationDetails { get; set; }
        public DateTime EstimationPlanStartDate { get; set; }
        public DateTime EstimationPlanEndDate { get; set; }
        public string EstimationRemarks { get; set; }
        public string TotalPriceRemarks { get; set; }
        public double EstimaionTotalPrice { get; set; }
        public int CreatorID { get; set; }
        public string CreateorFullName { get; set; }
        public string CreatorDepartment { get; set; }
        public string CreatorEmail { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}

using System;

namespace AMS.Repositories.DatabaseRepos.ProcurementApproval.Models
{
    public class GetProcurementApprovalResponse
    {
        public int ProcurementApprovalId { get; set; }
        public int EstimationId { get; set; }
        public string PAReferenceNo { get; set; }
        public string TitleOfPRorRFQ { get; set; }
        public string RFQReferenceNo { get; set; }
        public string PRReferenceNo { get; set; }
        public string NameOfRequester { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string RFQProcess { get; set; }
        public string SourcingMethod { get; set; }
        public string NameOfRecommendedSupplier { get; set; }
        public string PurchaseValue { get; set; }
        public string SavingAmount { get; set; }
        public string SavingType { get; set; }
        public int Created_By { get; set; }
        public DateTime Created_Date { get; set; }
        public int Updated_By { get; set; }
        public DateTime Updated_Date { get; set; }
    }
}
    
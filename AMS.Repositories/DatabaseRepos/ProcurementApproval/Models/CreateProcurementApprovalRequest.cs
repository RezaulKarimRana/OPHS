namespace AMS.Repositories.DatabaseRepos.ProcurementApproval.Models
{
    public class CreateProcurementApprovalRequest
    {
        public int EstimationId { get; set; }
        public string PAReferenceNo { get; set; }
        public string TitleOfPRorRFQ { get; set; }
        public string RFQReferenceNo { get; set; }
        public string PRReferenceNo { get; set; }
        public string NameOfRequester { get; set; }
        public int DepartmentId { get; set; }
        
        public string RFQProcess { get; set; }
        public string SourcingMethod { get; set; }
        public string NameOfRecommendedSupplier { get; set; }
        public string PurchaseValue { get; set; }
        public string SavingAmount { get; set; }
        public string SavingType { get; set; }
        public int Created_By { get; set; }

    }
}

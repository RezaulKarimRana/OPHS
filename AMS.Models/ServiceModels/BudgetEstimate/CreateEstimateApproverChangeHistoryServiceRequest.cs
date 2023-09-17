namespace AMS.Models.ServiceModels.BudgetEstimate
{
    public class CreateEstimateApproverChangeHistoryServiceRequest
    {
        public int Estimate_Id { get; set; }
        public int User_Id { get; set; }
        public int Priority { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
        public int RolePriority_Id { get; set; }
        public CreateEstimateApproverChangeHistoryServiceRequest()
        {
            Remarks = "";
            RolePriority_Id = 0;
        }
    }
}

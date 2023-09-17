namespace AMS.Models.ServiceModels.BudgetEstimate.BudgetApprover
{
    public class CreateApproverFeedBackServiceRequest
    {
        public int EstimateApprover_Id { get; set; }
        public int Estimation_Id { get; set; }
        public string FeedbackRemarks { get; set; }
        public int Status { get; set; }
    }
}

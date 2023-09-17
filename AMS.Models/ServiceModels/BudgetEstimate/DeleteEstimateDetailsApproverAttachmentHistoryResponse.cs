namespace AMS.Models.ServiceModels.BudgetEstimate
{
    public class DeleteEstimateDetailsApproverAttachmentHistoryResponse : ServiceResponse
    {
        public bool Success { get; set; }
        public DeleteEstimateDetailsApproverAttachmentHistoryResponse()
        {
            Success = true;
        }
    }
}

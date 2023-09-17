namespace AMS.Models.ServiceModels.BudgetEstimate.BudgetApprover
{
    public class GetApproverByEstimateAndUser
    {
        public int EstimationId { get; set; }
        public int UserId { get; set; }
    }
}

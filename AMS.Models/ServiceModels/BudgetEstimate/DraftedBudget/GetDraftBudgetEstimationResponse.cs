using AMS.Models.DomainModels;

namespace AMS.Models.ServiceModels.BudgetEstimate.DraftedBudget
{
    public class GetDraftBudgetEstimationResponse : ServiceResponse
    {
        public EstimationEntity Estimation { get; set; }
        public GetDraftBudgetEstimationResponse()
        {
            Estimation = new EstimationEntity();
        }
    }
}

using AMS.Models.CustomModels;

namespace AMS.Models.ServiceModels.BudgetEstimate
{
    public class SingleEstimationWithTypeResponse : ServiceResponse
    {
        public EstimationWithEstimationType Estimation { get; set; }
        public SingleEstimationWithTypeResponse()
        {
            Estimation = new EstimationWithEstimationType();
        }
    }
}

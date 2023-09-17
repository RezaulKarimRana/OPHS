using AMS.Models.ServiceModels.BudgetEstimate;
using System.Collections.Generic;
namespace AMS.Models.ViewModel
{
    public class AddBudgetEstimationMemoDTO
    {
        public EstimateMemoDTO EstimateMemo { get; set; }
        public List<CreateEstimateApproverServiceRequest> EstimateApproverList { get; set; }

        public AddBudgetEstimationMemoDTO()
        {
            EstimateMemo = new EstimateMemoDTO();
            EstimateApproverList = new List<CreateEstimateApproverServiceRequest>();
        }
    }
}

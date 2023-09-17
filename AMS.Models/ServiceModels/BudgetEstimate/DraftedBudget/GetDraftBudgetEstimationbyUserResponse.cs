using AMS.Models.CustomModels;
using System.Collections.Generic;

namespace AMS.Models.ServiceModels.BudgetEstimate.DraftedBudget
{
    public class GetDraftBudgetEstimationbyUserResponse : ServiceResponse
    {
        public List<DraftedBudgetEstimationByUser> DraftedBudgest { get; set; }

        public GetDraftBudgetEstimationbyUserResponse()
        {
            DraftedBudgest = new List<DraftedBudgetEstimationByUser>();
        }
    }
}

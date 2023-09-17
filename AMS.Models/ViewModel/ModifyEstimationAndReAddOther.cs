using AMS.Models.ServiceModels.BudgetEstimate;
using System.Collections.Generic;

namespace AMS.Models.ViewModel
{
    public class ModifyEstimationAndReAddOther
    {
        public int EstimationId { get; set; }
        public int EstimationStatus { get; set; }
        public BudgetEstimationUpdateRequest BudgetEstimationUpdateRequest { get; set; }
        public List<int> AttachmentsToRemove { get; set; }
        public AddBudgetEstimation ReAddBudgetDetailsAndOthers { get; set; }
        public string TotalPriceRemarks { get; set; }
    }
}

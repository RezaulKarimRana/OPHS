using AMS.Models.CustomModels;
using AMS.Models.ServiceModels.BudgetEstimate;
using System.Collections.Generic;

namespace AMS.Models.ViewModel
{
    public class AddBudgetEstimation
    {
        public CreateEstimateRequest Estimation { get; set; }
        public List<CreateEstimateApproverServiceRequest> EstimateApproverList { get; set; }
        public List<CreateEstimateDetailsServiceRequest> EstimateDetails { get; set; }
        public List<CreateDepartmentWiseSummaryServiceRequest> DepartmentWiseSummary { get; set; }
        public List<DepartWiseSummaryDetailsForSettledEstimationIncludeTADA> DepartmentWiseRunningSummary { get; set; }
        public List<ParticularWiseSummaryDetailsForSettledEstimationIncludeTADA> ParticularWiseRunningSummary { get; set; }
        public List<CreateParticularWiseSummaryServiceRequest> ParticularWiseSummary { get; set; }
        public CreateProcurementApprovalServiceRequest ProcurementApprovalRequest { get; set; }
        public List<LoadApproverFeedBackDetails> EstimateApproverFeedBacks { get; set; }
    }
}

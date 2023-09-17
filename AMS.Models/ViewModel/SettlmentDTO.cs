using System;
using System.Collections.Generic;
using System.Text;
using AMS.Models.CustomModels;
using AMS.Models.ServiceModels.BudgetEstimate;
using AMS.Models.ServiceModels.Settlement;

namespace AMS.Models.ViewModel
{
    public class SettlmentDTO
    {
        public CreateSettlementRequest Settlement { get; set; }
        public List<CreateSettlementApproverRequest> SettlementApproverList { get; set; }
        public List<CreateSettlementItemRequest> SettlementItems { get; set; }
        
        //public List<CreateEstimateDetailsServiceRequest> EstimateDetails { get; set; }
        //public List<CreateDepartmentWiseSummaryServiceRequest> DepartmentWiseSummary { get; set; }
        //public List<CreateParticularWiseSummaryServiceRequest> ParticularWiseSummary { get; set; }
        //public CreateProcurementApprovalServiceRequest ProcurementApprovalRequest { get; set; }
        //public List<LoadApproverFeedBackDetails> EstimateApproverFeedBacks { get; set; }
    }
}

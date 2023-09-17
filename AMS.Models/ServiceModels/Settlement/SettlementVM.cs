using System;
using System.Collections.Generic;
using System.Text;

namespace AMS.Models.ServiceModels.Settlement
{
    public class SettlementVM
    {
        public int EstimationId { get; set; }
        public int SettlementId { get; set; }
       
        public string Status { get; set; }
        public string ProjectName { get; set; }
        public string EstimateIdentifier { get; set; }
        public string Subject { get; set; }
        public string Objective { get; set; }
        public string Details { get; set; }
        public string PlanStartDate { get; set; }
        public string PlanEndDate { get; set; }
        public string Remarks { get; set; }
        public string IsItFinalSettlement { get; set; }
        public Double TotalBudgedPrice { get; set; }
        public Double AllowableBudget { get; set; }
        public Double AlreadySettle { get; set; }
        public Double SettleAmount { get; set; }
        public int Priority { get; set; }
        public string CreatorFullName { get; set; }
        public int  SettlementInitiatorUserId { get; set; }
        public string SettlementInitiateDate { get; set; }
        public string SettlementApprovalList { get; set; }
        public int TotalRow { get; set; }
    }
}

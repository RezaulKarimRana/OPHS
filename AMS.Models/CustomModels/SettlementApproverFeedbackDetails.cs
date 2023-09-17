using System;
using System.Collections.Generic;
using System.Text;

namespace AMS.Models.CustomModels
{
    
    public class SettlementApproverFeedbackDetails
    {
        public int SettlementApproverId { get; set; }
        public int SettlementId { get; set; }
        public int ApproverUserId { get; set; }
        public string ApproverUserName { get; set; }
        public string ApproverFullName { get; set; }
        public string FeedBack { get; set; }
        public int SettlementStatus { get; set; }
        public string EstimateApproverStatusString { get; set; }
        public string FeedBackDate { get; set; }
    }
}

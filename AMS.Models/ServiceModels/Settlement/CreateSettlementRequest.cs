using System;
using System.Collections.Generic;
using System.Text;
using AMS.Models.DomainModels;

namespace AMS.Models.ServiceModels.Settlement
{
    public class CreateSettlementRequest
    {
        public int Id { get; set; }
        public int EstimationId { get; set; }
        public int CurrentApprovalUserId { get; set; }
        public int CurrentApprovalUserRolePiority { get; set; }
        public int IsItFinalSetttlement { get; set; }
        public int Status { get; set; }
        public string SettlementNote { get; set; }
        public double TotalAmount { get; set; }
        public int CreatedBy { get; set; }
        public string EstimateIdentifier { get; set; }
        public DateTime CreationDate { get; set; }

        public string SettlementInitiatorNameDept { get; set; }

        public List<EstimateSettlementAttachmentsEntity> EstimateSettlementAttachments { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace AMS.Models.ServiceModels.FundDisburse
{
    public class FundDisburseVM
    {
        public string EstimateIdentifier { get; set; }
        public string EstimationSubject { get; set; }
        
       
        public string RequisitionStatus { get; set; }
        public Double AllowableBudget { get; set; }
        public DateTime FundRequisitionDate { get; set; }
        public DateTime ProposedDisburseDate { get; set; }
        public DateTime FundAvailableDate { get; set; }
        public Double FundRequisitionAmount { get; set; }
        public Double FundDisburseAmount { get; set; }
        public string RequisitionType { get; set; }

        public int EstimatationId { get; set; }
        public int DisburseStatus { get; set; }
        public int FundRequisitionId { get; set; }
        public int FundDisburseId { get; set; }
        public int FundSenderUserId { get; set; }
        public string FundSenderName { get; set; }
        public string FundReceiverName { get; set; }


        public string RemarksByFinance { get; set; }
        public string RemarksByFundReceiver { get; set; }
        public string FundRequisitionRemarksByFundRequestor { get; set; }
        public string FundDisburseRemarksByFinance { get; set; }

        public Double FundRequested { get; set; }
        public Double DepartmentalFundRequested { get; set; }
        public Double AlreadyDisburseAmount { get; set; }
        public Double RemainingBudget { get; set; }
  
        public Double ReceivedAmountByRequestor { get; set; }

       
        public string RequistionDepartmentName { get; set; }
    }
}

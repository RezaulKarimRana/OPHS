using System;
using System.Collections.Generic;
using System.Text;


namespace AMS.Models.DomainModels
{
    public class FundRequisition : BaseEntity
    {
        public int Type { get; set; } /*Fund or Payment*/
        public double Amount { get; set; }
        public DateTime ProposedDisburseDate { get; set; }
        public String Remarks { get; set; }
        public int RequisitionStatus { get; set; }
        public int EstimationId { get; set; }

    }
}

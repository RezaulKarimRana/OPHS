using System;

namespace AMS.Models.DomainModels
{
    public class FundDisburse : BaseEntity
    {
        public double DisburseAmount { get; set; }
        public double ReceivedAmount { get; set; }
        public int Status { get; set; }
        public bool isRollBack { get; set; }
        public int FundRequisitionId { get; set; }
        public DateTime FundAvailableDate { get; set; }
        public string RemarksByFinance { get; set; }
        public string RemarksByFundReceiver { get; set; }

    }
}

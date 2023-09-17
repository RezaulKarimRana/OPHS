using System;
using System.Collections.Generic;
using System.Text;

namespace AMS.Models.DomainModels
{
    public class EstimateSettlementAttachmentsEntity : BaseEntity
    {
        public string URL { get; set; }
        public string FileName { get; set; }
        public int EstimateSettlement_Id { get; set; }
    }
}

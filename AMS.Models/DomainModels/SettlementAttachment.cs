using System;
using System.Collections.Generic;
using System.Text;

namespace AMS.Models.DomainModels
{
    public class SettlementAttachment: BaseEntity
    {
        public string URL { get; set; }
        public string FileName { get; set; }
        public int SettlementId { get; set; }
    }
}

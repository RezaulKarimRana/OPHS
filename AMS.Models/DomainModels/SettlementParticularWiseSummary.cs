using System;
using System.Collections.Generic;
using System.Text;

namespace AMS.Models.DomainModels
{
    public class SettlementParticularWiseSummary : BaseEntity
    {
        public int Particular_Id { get; set; }
        public double TotalPrice { get; set; }
        public int Settlement_Id { get; set; }

    }
}

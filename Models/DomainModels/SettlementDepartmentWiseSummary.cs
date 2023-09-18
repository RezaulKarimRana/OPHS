using System;
using System.Collections.Generic;
using System.Text;

namespace Models.DomainModels
{
    public class SettlementDepartmentWiseSummary : BaseEntity
    {
        public int Department_Id { get; set; }
        public double TotalPrice { get; set; }
        public int Settlement_Id { get; set; }

    }
}

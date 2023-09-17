using System;
using System.Collections.Generic;
using System.Text;

namespace AMS.Models.DomainModels
{
    public class FundRequisitionConfig : BaseEntity
    {
        public int isEligibleForParking { get; set; }
        public int isDeductFromCalculation { get; set; }
        public int Department_Id { get; set; }
    }
}

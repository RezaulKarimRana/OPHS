using System;
using System.Collections.Generic;
using System.Text;

namespace AMS.Models.CustomModels
{
    public  class FundDisburseHistory
    {
        public int FundRequisitionId { get; set; }
        public string DepartmentName { get; set; }
        public DateTime RequisitionDate { get; set; }
        public Double RequisitionAmount { get; set; }
        public Double FundDisburseId { get; set; }
        public Double DisburseAmount { get; set; }
        public Double TotalDisburseAmount { get; set; }

    }
}

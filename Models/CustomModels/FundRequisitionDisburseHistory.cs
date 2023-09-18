using System;
using System.Collections.Generic;
using System.Text;

namespace Models.CustomModels
{
    public class FundRequisitionDisburseHistory
    {
        public int Sl { get; set; }
        public int FundRequisitionId { get; set; }
        public string FundType { get; set; }
        public string DepartmentName { get; set; }
        public String RequisitionDate { get; set; }
        public String ProposedDisburseDate { get; set; }
        public Double RequisitionAmount { get; set; }
        public string DisburseHistory { get; set; }
    }
}

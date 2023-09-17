using System;
using System.Collections.Generic;
using System.Text;

namespace AMS.Models.ServiceModels.FundRequisition
{
   public class FundRequisitionVM
    {
        public int FundRequisitionId { get; set; }
        public string RequisitionType { get; set; }
        public string RequisitionStatus { get; set; }
       
        public string EstimateIdentifier { get; set; }
        public int EstimatationId { get; set; }
        public string Subject { get; set; }
       
        public string Remarks { get; set; }
        public string ProposedDisburseDate { get; set; }
       
        public Double Amount { get; set; }
        public Double AlreadyDisburseAmount { get; set; }
        public Double AllowableBudget { get; set; }
        public Double TotalAllowableBudget { get; set; }
        public Double DepartmentWiseTotalAllowableBudget { get; set; }
       
        public int TotalRequisitionAmount { get; set; }
        public double DepartmentWiseTotalRequisition { get; set; }
        public int FundRequested { get; set; }
        public int DepartmentalFundRequested { get; set; }
        public int RemainingBudget { get; set; }
        public double DepartmentWiseRemainingBudget { get; set; }
        public int TotalReceived { get; set; }

        public string CreateorFullName { get; set; }
        public int FundRequisitionCreatedBy { get; set; }
        public string RequistionDepartmentName { get; set; }
        public string FundRejectorName { get; set; }
    }
}

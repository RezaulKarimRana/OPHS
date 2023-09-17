using AMS.Repositories.DatabaseRepos.Common;
using System;

namespace AMS.Repositories.DatabaseRepos.EstimationRepo.Models
{
    public class CreateEstimationRequest : CreatedBy
    {
        public int EstimateType { get; set; }
        public int CurrencyType { get; set; }
        public string Status { get; set; }
        public string SystemID { get; set; }
        public int Project_Id { get; set; }
        //public string UniqueIdentifier { get; set; }
        public string Subject { get; set; }
        public string Objective { get; set; }
        public string Details { get; set; }
        public string PlanStartDate { get; set; }
        public string PlanEndDate { get; set; }
        public string Remarks { get; set; }
        public string TotalPriceRemarks { get; set; }
        public string DepartmentName { get; set; }
        public Double TotalPrice { get; set; }
    }
}

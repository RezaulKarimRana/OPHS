using AMS.Repositories.DatabaseRepos.Common;

namespace AMS.Repositories.DatabaseRepos.DepartmentWiseSummaryHistoryRepo.Models
{
    public class CreateDepartmentWiseSummaryHistoryRequest : CreatedBy
    {
        public int Department_Id { get; set; }
        public double TotalPrice { get; set; }
        public int Estimate_Id { get; set; }
        //public int DepartmentWiseSummary_Id { get; set; }
    }
}

using AMS.Repositories.DatabaseRepos.Common;

namespace AMS.Repositories.DatabaseRepos.DepartmentWiseSummaryRepo.Models
{
    public class CreateDepartmentWiseSummaryRequest : CreatedBy
    {
        public int Department_Id { get; set; }
        public double TotalPrice { get; set; }
        public int Estimate_Id { get; set; }
    }
}

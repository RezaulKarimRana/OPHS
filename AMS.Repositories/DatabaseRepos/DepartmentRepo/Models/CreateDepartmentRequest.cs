using AMS.Repositories.DatabaseRepos.Common;

namespace AMS.Repositories.DatabaseRepos.DepartmentRepo.Models
{
    public class CreateDepartmentRequest : CreatedBy
    {
        public string Name { get; set; }
        public bool CanEdit { get; set; }
    }
}

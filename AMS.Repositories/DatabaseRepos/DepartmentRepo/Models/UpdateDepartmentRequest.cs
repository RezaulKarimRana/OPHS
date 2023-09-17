using AMS.Repositories.DatabaseRepos.Common;

namespace AMS.Repositories.DatabaseRepos.DepartmentRepo.Models
{
    public class UpdateDepartmentRequest : UpdatedBy
    {
        public int Name { get; set; }
        public bool CanEdit { get; set; }
    }
}

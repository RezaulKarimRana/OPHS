using AMS.Repositories.DatabaseRepos.Common;

namespace AMS.Repositories.DatabaseRepos.RolePriorityRepo.Models
{
    public class UpdateRoleRequest : UpdatedBy
    {
        public int Name { get; set; }
    }
}

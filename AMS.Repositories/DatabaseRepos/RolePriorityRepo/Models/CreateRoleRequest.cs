using AMS.Repositories.DatabaseRepos.Common;

namespace AMS.Repositories.DatabaseRepos.RolePriorityRepo.Models
{
    public class CreateRoleRequest : CreatedBy
    {
        public string Name { get; set; }
    }
}

using AMS.Models.DomainModels;
using AMS.Repositories.DatabaseRepos.RolePriorityRepo.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AMS.Repositories.DatabaseRepos.RolePriorityRepo.Contracts
{
    public interface IRolePriorityRepo
    {
        Task<int> CreateRole(CreateRoleRequest request);
        Task<List<RolePriorityEntity>> GetAllRoles();
        Task<RolePriorityEntity> GetSingleRole(int id);
        Task UpdateRole(UpdateRoleRequest request);
        Task DeleteRole(DeleteRoleRequest request);
    }
}

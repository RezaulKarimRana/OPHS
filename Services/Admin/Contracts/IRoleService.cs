using System.Threading.Tasks;
using Models.ServiceModels.Admin.Roles;

namespace AMS.Services.Admin.Contracts
{
    public interface IRoleService
    {
        Task<GetRolesResponse> GetRoles();

        Task<DisableRoleResponse> DisableRole(DisableRoleRequest request);

        Task<EnableRoleResponse> EnableRole(EnableRoleRequest request);

        Task<GetRoleResponse> GetRole(GetRoleRequest request);

        Task<UpdateRoleResponse> UpdateRole(UpdateRoleRequest request);

        Task<CreateRoleResponse> CreateRole(CreateRoleRequest request);
    }
}

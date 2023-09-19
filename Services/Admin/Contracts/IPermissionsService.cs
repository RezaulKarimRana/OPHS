using System.Threading.Tasks;
using Models.ServiceModels.Admin.Permissions;

namespace Services.Admin.Contracts
{
    public interface IPermissionsService
    {
        Task<GetPermissionsResponse> GetPermissions();

        Task<GetPermissionResponse> GetPermission(GetPermissionRequest request);

        Task<UpdatePermissionResponse> UpdatePermission(UpdatePermissionRequest request);

        Task<CreatePermissionResponse> CreatePermission(CreatePermissionRequest request);
    }
}

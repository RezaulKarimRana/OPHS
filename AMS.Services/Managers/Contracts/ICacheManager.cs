using System.Collections.Generic;
using System.Threading.Tasks;
using AMS.Infrastructure.Configuration.Models;
using AMS.Models.DomainModels;

namespace AMS.Services.Managers.Contracts
{
    public interface ICacheManager
    {
        Task<ApplicationConfiguration> Configuration();

        Task<ApplicationConfiguration_Javascript> Configuration_Javascript();

        Task<List<RoleEntity>> Roles();

        Task<List<UserRoleEntity>> UserRoles();

        Task<List<RolePermissionEntity>> RolePermissions();

        Task<List<PermissionEntity>> Permissions();

        Task<List<SessionEventEntity>> SessionEvents();

        void Remove(string key);
    }
}

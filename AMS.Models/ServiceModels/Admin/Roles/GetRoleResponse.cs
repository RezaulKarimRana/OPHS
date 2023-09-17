using System.Collections.Generic;
using AMS.Models.DomainModels;

namespace AMS.Models.ServiceModels.Admin.Roles
{
    public class GetRoleResponse : ServiceResponse
    {
        public RoleEntity Role { get; set; }

        public List<PermissionEntity> Permissions { get; set; }

        public GetRoleResponse()
        {
            Permissions = new List<PermissionEntity>();
        }
    }
}

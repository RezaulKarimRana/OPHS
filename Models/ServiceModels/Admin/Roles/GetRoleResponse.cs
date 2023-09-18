using System.Collections.Generic;
using Models.DomainModels;

namespace Models.ServiceModels.Admin.Roles
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

using System.Collections.Generic;
using Models.DomainModels;

namespace Models.ServiceModels.Admin.Permissions
{
    public class GetPermissionsResponse : ServiceResponse
    {
        public List<PermissionEntity> Permissions { get; set; }

        public GetPermissionsResponse()
        {
            Permissions = new List<PermissionEntity>();
        }
    }
}

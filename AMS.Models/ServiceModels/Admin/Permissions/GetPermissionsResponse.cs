using System.Collections.Generic;
using AMS.Models.DomainModels;

namespace AMS.Models.ServiceModels.Admin.Permissions
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

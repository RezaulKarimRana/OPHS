using AMS.Models.DomainModels;

namespace AMS.Models.ServiceModels.Admin.Permissions
{
    public class GetPermissionResponse : ServiceResponse
    {
        public PermissionEntity Permission { get; set; }
    }
}

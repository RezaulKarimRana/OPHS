using Models.DomainModels;

namespace Models.ServiceModels.Admin.Permissions
{
    public class GetPermissionResponse : ServiceResponse
    {
        public PermissionEntity Permission { get; set; }
    }
}

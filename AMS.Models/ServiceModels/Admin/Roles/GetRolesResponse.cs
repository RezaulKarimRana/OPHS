using System.Collections.Generic;
using AMS.Models.DomainModels;

namespace AMS.Models.ServiceModels.Admin.Roles
{
    public class GetRolesResponse : ServiceResponse
    {
        public List<RoleEntity> Roles { get; set; }

        public GetRolesResponse()
        {
            Roles = new List<RoleEntity>();
        }
    }
}

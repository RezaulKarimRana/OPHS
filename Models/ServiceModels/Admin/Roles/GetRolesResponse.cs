using System.Collections.Generic;
using Models.DomainModels;

namespace Models.ServiceModels.Admin.Roles
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

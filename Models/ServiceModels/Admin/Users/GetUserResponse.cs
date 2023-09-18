using System.Collections.Generic;
using Models.DomainModels;

namespace Models.ServiceModels.Admin.Users
{
    public class GetUserResponse : ServiceResponse
    {
        public UserEntity User { get; set; }

        public List<RoleEntity> Roles { get; set; }

        public GetUserResponse()
        {
            Roles = new List<RoleEntity>();
        }
    }
}

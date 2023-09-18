using System.Collections.Generic;
using Models.DomainModels;

namespace Models.ServiceModels.Account
{
    public class GetProfileResponse : ServiceResponse
    {
        public UserEntity User { get; set; }

        public List<RoleEntity> Roles { get; set; }

        public GetProfileResponse()
        {
            Roles = new List<RoleEntity>();
        }
    }
}

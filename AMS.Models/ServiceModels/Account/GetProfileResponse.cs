using System.Collections.Generic;
using AMS.Models.DomainModels;

namespace AMS.Models.ServiceModels.Account
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

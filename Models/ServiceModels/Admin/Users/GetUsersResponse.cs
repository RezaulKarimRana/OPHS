using System.Collections.Generic;
using Models.DomainModels;

namespace Models.ServiceModels.Admin.Users
{
    public class GetUsersResponse : ServiceResponse
    {
        public List<UserEntity> Users { get; set; }

        public GetUsersResponse()
        {
            Users = new List<UserEntity>();
        }
    }
}

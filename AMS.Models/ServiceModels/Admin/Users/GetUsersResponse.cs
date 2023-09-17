using System.Collections.Generic;
using AMS.Models.DomainModels;

namespace AMS.Models.ServiceModels.Admin.Users
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

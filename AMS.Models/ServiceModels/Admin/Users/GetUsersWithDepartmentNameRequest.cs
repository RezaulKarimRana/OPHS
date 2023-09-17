using AMS.Models.CustomModels;
using System.Collections.Generic;

namespace AMS.Models.ServiceModels.Admin.Users
{
    public class GetUsersWithDepartmentNameRequest : ServiceResponse
    {
        public IList<GetUsersWithDepartmentName> Users { get; set; }

        public GetUsersWithDepartmentNameRequest()
        {
            Users = new List<GetUsersWithDepartmentName>();
        }
    }
}

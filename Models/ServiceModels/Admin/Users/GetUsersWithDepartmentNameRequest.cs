using Models.CustomModels;
using System.Collections.Generic;

namespace Models.ServiceModels.Admin.Users
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

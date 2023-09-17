using AMS.Models.DomainModels;
using System.Collections.Generic;

namespace AMS.Models.ServiceModels.Admin.Department
{
    public class GetAllDepartmentsResponse : ServiceResponse
    {
        public IList<DepartmentEntity> Departments { get; set; }
        public GetAllDepartmentsResponse()
        {
            Departments = new List<DepartmentEntity>();
        }
    }
}

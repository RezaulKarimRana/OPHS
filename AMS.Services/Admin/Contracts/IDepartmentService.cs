using AMS.Models.DomainModels;
using AMS.Models.ServiceModels.Admin.Department;
using System.Threading.Tasks;

namespace AMS.Services.Admin.Contracts
{
    public interface IDepartmentService
    {
        Task<GetAllDepartmentsResponse> GetAllDepartments();
        Task<GetAllDepartmentsResponse> GetAllDepartmentsJoiningUser();
        Task<GetAllDepartmentsResponse> GetAllDepartmentsJoinUserByConfiguration();
        Task<DepartmentEntity> GetById(int id);
    }
}

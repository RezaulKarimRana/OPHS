using AMS.Models.DomainModels;
using AMS.Repositories.DatabaseRepos.DepartmentRepo.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AMS.Repositories.DatabaseRepos.DepartmentRepo.Contracts
{
    public interface IDepartmentRepo
    {
        Task<int> CreateDepartment(CreateDepartmentRequest request);
        Task<List<DepartmentEntity>> GetAllDepartments();
        Task<List<DepartmentEntity>> GetAllDepartmentsJoinUserTable();
        Task<List<DepartmentEntity>> GetAllDepartmentsJoinUserByConfiguration();
        Task<DepartmentEntity> GetSingleDepartment(int id);
        Task UpdateDepartment(UpdateDepartmentRequest request);
        Task DeleteDepartment(DeleteDepartmentRequest request);

    }
}

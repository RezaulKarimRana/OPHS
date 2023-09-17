using AMS.Models.DomainModels;
using AMS.Models.ServiceModels.Admin.Department;
using AMS.Repositories.UnitOfWork.Contracts;
using AMS.Services.Admin.Contracts;
using System;
using System.Threading.Tasks;

namespace AMS.Services.Admin
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IUnitOfWorkFactory _uowFactory;

        public DepartmentService(IUnitOfWorkFactory uowFactory)
        {
            _uowFactory = uowFactory;
        }
        public async Task<GetAllDepartmentsResponse> GetAllDepartments()
        {
            var response = new GetAllDepartmentsResponse();

            using var uow = _uowFactory.GetUnitOfWork();
            response.Departments = await uow.DepartmentRepo.GetAllDepartments();
            uow.Commit();

            return response;
        }

        public async Task<GetAllDepartmentsResponse> GetAllDepartmentsJoiningUser()
        {
            var response = new GetAllDepartmentsResponse();

            using var uow = _uowFactory.GetUnitOfWork();
            response.Departments = await uow.DepartmentRepo.GetAllDepartmentsJoinUserTable();
            uow.Commit();

            return response;
        } 
        public async Task<GetAllDepartmentsResponse> GetAllDepartmentsJoinUserByConfiguration()
        {
            var response = new GetAllDepartmentsResponse();

            using var uow = _uowFactory.GetUnitOfWork();
            response.Departments = await uow.DepartmentRepo.GetAllDepartmentsJoinUserByConfiguration();
            uow.Commit();

            return response;
        }
        

        public async Task<DepartmentEntity> GetById(int id)
        {
            try
            {
                DepartmentEntity response = null;
                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    response = await uow.DepartmentRepo.GetSingleDepartment(id);
                }
                return response;
            }
            catch (Exception e) { throw; }
        }
    }
}

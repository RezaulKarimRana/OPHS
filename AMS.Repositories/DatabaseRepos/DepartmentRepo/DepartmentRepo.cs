using AMS.Infrastructure.Adapters;
using AMS.Infrastructure.Configuration.Models;
using AMS.Models.DomainModels;
using AMS.Repositories.DatabaseRepos.DepartmentRepo.Contracts;
using AMS.Repositories.DatabaseRepos.DepartmentRepo.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace AMS.Repositories.DatabaseRepos.DepartmentRepo
{
    public class DepartmentRepo : BaseSQLRepo, IDepartmentRepo
    {
        private readonly IDbConnection _connection;
        private readonly IDbTransaction _transaction;

        public DepartmentRepo(IDbConnection connection, IDbTransaction transaction, ConnectionStringSettings connectionStringsSettings)
            : base(connectionStringsSettings)
        {
            _connection = connection;
            _transaction = transaction;
        }
        public async Task<int> CreateDepartment(CreateDepartmentRequest request)
        {
            var sqlStoredProc = "sp_department_create";

            var response = await DapperAdapter.GetFromStoredProcAsync<int>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: request,
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction
                );

            if (response == null || response.First() == 0)
            {
                throw new Exception("No items have been created");
            }
            return response.FirstOrDefault();
        }

        public async Task DeleteDepartment(DeleteDepartmentRequest request)
        {
            var sqlStoredProc = "sp_department_delete";

            var response = await DapperAdapter.GetFromStoredProcAsync<int>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: request,
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction
                );

            if (response == null || response.First() == 0)
            {
                throw new Exception("No items have been deleted");
            }
        }

        public async Task<List<DepartmentEntity>> GetAllDepartments()
        {
            var sqlStoredProc = "sp_all_department_get";

            var response = await DapperAdapter.GetFromStoredProcAsync<DepartmentEntity>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: new { },
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction
                );

            return response.ToList();
        }

        public async Task<List<DepartmentEntity>> GetAllDepartmentsJoinUserTable()
        {
            var sqlStoredProc = "sp_load_department_joining_user_table";

            var response = await DapperAdapter.GetFromStoredProcAsync<DepartmentEntity>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: new { },
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction
                );

            return response.ToList();
        }
        public async Task<List<DepartmentEntity>> GetAllDepartmentsJoinUserByConfiguration()
        {
            var sqlStoredProc = "sp_load_department_joining_user_table_by_configuration";

            var response = await DapperAdapter.GetFromStoredProcAsync<DepartmentEntity>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: new { },
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction
                );

            return response.ToList();
        }

        public async Task<DepartmentEntity> GetSingleDepartment(int id)
        {
            var sqlStoredProc = "sp_get_dept_by_id";

            var response = await DapperAdapter.GetFromStoredProcAsync<DepartmentEntity>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: new { Id = id },
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction
                );

            return response.FirstOrDefault();
        }

        public async Task UpdateDepartment(UpdateDepartmentRequest request)
        {
            var sqlStoredProc = "sp_department_update";

            var response = await DapperAdapter.GetFromStoredProcAsync<int>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: request,
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction
                );

            if (response == null || response.First() == 0)
            {
                throw new Exception("No items have been updated");
            }
        }
    }
}

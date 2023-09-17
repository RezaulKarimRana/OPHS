using AMS.Infrastructure.Adapters;
using AMS.Infrastructure.Configuration.Models;
using AMS.Models.DomainModels;
using AMS.Repositories.DatabaseRepos.ProjectRepo.Contracts;
using AMS.Repositories.DatabaseRepos.ProjectRepo.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace AMS.Repositories.DatabaseRepos.ProjectRepo
{
    public class ProjectRepo : BaseSQLRepo, IProjectRepo
    {
        private readonly IDbConnection _connection;
        private readonly IDbTransaction _transaction;

        public ProjectRepo(IDbConnection connection, IDbTransaction transaction, ConnectionStringSettings connectionStringsSettings)
            : base(connectionStringsSettings)
        {
            _connection = connection;
            _transaction = transaction;
        }

        public async Task<int> CreateProject(CreateProjectRequest request)
        {
            var sqlStoredProc = "sp_project_create";

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

        public async Task DeleteProject(DeleteProjectRequest request)
        {
            var sqlStoredProc = "sp_project_delete";

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

        public async Task<List<ProjectEntity>> GetAllProject()
        {
            var sqlStoredProc = "sp_all_project_get";

            var response = await DapperAdapter.GetFromStoredProcAsync<ProjectEntity>
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

        public async Task<ProjectEntity> GetSingleProject(int id)
        {
            var sqlStoredProc = "sp_single_project_get";

            var response = await DapperAdapter.GetFromStoredProcAsync<ProjectEntity>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: new { EsimtationId = id },
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction
                );

            return response.FirstOrDefault();
        }

        public async Task UpdateProject(UpdateProjectRequest request)
        {
            var sqlStoredProc = "sp_project_update";

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

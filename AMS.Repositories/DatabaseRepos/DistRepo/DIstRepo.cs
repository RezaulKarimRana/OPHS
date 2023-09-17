using AMS.Infrastructure.Adapters;
using AMS.Infrastructure.Configuration.Models;
using AMS.Models.DomainModels;
using AMS.Repositories.DatabaseRepos.DistRepo.Contracts;
using AMS.Repositories.DatabaseRepos.DistRepo.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace AMS.Repositories.DatabaseRepos.DistRepo
{
    public class DIstRepo : BaseSQLRepo, IDistRepo
    {
        private readonly IDbConnection _connection;
        private readonly IDbTransaction _transaction;

        public DIstRepo(IDbConnection connection, IDbTransaction transaction, ConnectionStringSettings connectionStringsSettings)
            : base(connectionStringsSettings)
        {
            _connection = connection;
            _transaction = transaction;
        }

        public async Task<int> CreateDist(CreateDistRequest request)
        {
            var sqlStoredProc = "sp_dist_create";

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

        public async Task DeleteDist(DeleteDistRequest request)
        {
            var sqlStoredProc = "sp_dist_delete";

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

        public async Task<List<DistEntity>> GetAllDist()
        {
            var sqlStoredProc = "sp_all_dist_get";

            var response = await DapperAdapter.GetFromStoredProcAsync<DistEntity>
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

        public async Task<DistEntity> GetSingleDist(int id)
        {
            var sqlStoredProc = "sp_get_dist_by_id";

            var response = await DapperAdapter.GetFromStoredProcSingleAsync<DistEntity>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: new { @id = id },
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction
                );

            return response;
        }

        public async Task UpdateDist(UpdateDistRequest request)
        {
            var sqlStoredProc = "sp_dist_update";

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

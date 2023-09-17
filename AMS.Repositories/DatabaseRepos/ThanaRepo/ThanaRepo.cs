using AMS.Infrastructure.Adapters;
using AMS.Infrastructure.Configuration.Models;
using AMS.Models.CustomModels;
using AMS.Models.DomainModels;
using AMS.Repositories.DatabaseRepos.ThanaRepo.Contracts;
using AMS.Repositories.DatabaseRepos.ThanaRepo.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace AMS.Repositories.DatabaseRepos.ThanaRepo
{
    public class ThanaRepo : BaseSQLRepo, IThanaRepo
    {
        private readonly IDbConnection _connection;
        private readonly IDbTransaction _transaction;

        public ThanaRepo(IDbConnection connection, IDbTransaction transaction, ConnectionStringSettings connectionStringsSettings)
            : base(connectionStringsSettings)
        {
            _connection = connection;
            _transaction = transaction;
        }
        public async Task<int> CreateThana(CreateThanaRequest request)
        {
            var sqlStoredProc = "sp_thana_create";

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

        public async Task DeleteThana(DeleteThanaRequest request)
        {
            var sqlStoredProc = "sp_thana_delete";

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

        public async Task<List<ThanaEntity>> GetAllThana()
        {
            var sqlStoredProc = "sp_all_thana_get";

            var response = await DapperAdapter.GetFromStoredProcAsync<ThanaEntity>
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

        public async Task<ThanaEntity> GetSingleThana(int id)
        {
            var sqlStoredProc = "sp_get_thana_by_id";

            var response = await DapperAdapter.GetFromStoredProcAsync<ThanaEntity>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: new { @id = id },
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction
                );

            return response.FirstOrDefault();
        }

        public async Task<IList<ThanaEntity>> GetThanaByDistId(int DistId)
        {
            var sqlStoredProc = "sp_thana_by_distId_get";

            if (DistId == 0)
            {
                var query = "SELECT Name, ID FROM Thana where is_deleted = 0";
                try
                {
                    var res = await DapperAdapter.ExecuteDynamicSql<ThanaEntity>(
                        query,
                        dbconnectionString: DefaultConnectionString,
                        sqltimeout: DefaultTimeOut,
                        dbconnection: _connection,
                        dbtransaction: _transaction
                     );
                    return res.ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return new List<ThanaEntity>();
                }
            }

            var response = await DapperAdapter.GetFromStoredProcAsync<ThanaEntity>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: new { DistId },
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction
                );

            return response.ToList();
        }

        public async Task UpdateThana(UpdateThanaRequest request)
        {
            var sqlStoredProc = "sp_thana_update";

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

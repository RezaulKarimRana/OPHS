using AMS.Infrastructure.Adapters;
using AMS.Infrastructure.Configuration.Models;
using AMS.Models.DomainModels;
using AMS.Models.ViewModel;
using AMS.Repositories.DatabaseRepos.ParticularRepo.Contracts;
using AMS.Repositories.DatabaseRepos.ParticularRepo.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace AMS.Repositories.DatabaseRepos.ParticularRepo
{
    public class ParticularRepo : BaseSQLRepo, IParticularRepo
    {
        private readonly IDbConnection _connection;
        private readonly IDbTransaction _transaction;

        public ParticularRepo(IDbConnection connection, IDbTransaction transaction, ConnectionStringSettings connectionStringsSettings)
            : base(connectionStringsSettings)
        {
            _connection = connection;
            _transaction = transaction;
        }

        public async Task<int> CreateParticular(CreateParticularRequest request)
        {
            var sqlStoredProc = "sp_particular_create";

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

        public async Task DeleteParticular(DeleteParticularRequest request)
        {
            var sqlStoredProc = "sp_particular_delete";

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

        public async Task<List<ParticularEntity>> GetAllParticular()
        {
            var sqlStoredProc = "sp_all_particular_get";

            var response = await DapperAdapter.GetFromStoredProcAsync<ParticularEntity>
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
        public async Task<List<NameIdPairModel>> GetAllAsNameIdPair()
        {
            var sqlStoredProc = "sp_all_particular_get_nameIdPair";

            var response = await DapperAdapter.GetFromStoredProcAsync<NameIdPairModel>
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

        public async Task<ParticularEntity> GetSingleParticular(int id)
        {
            var sqlStoredProc = "sp_get_particular_by_id";

            var response = await DapperAdapter.GetFromStoredProcAsync<ParticularEntity>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: new { @id = id},
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction
                );

            return response.FirstOrDefault();
        }

        public async Task UpdateParticular(UpdateParticularRequest request)
        {
            var sqlStoredProc = "sp_particular_update";

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

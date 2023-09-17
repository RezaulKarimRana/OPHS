using AMS.Infrastructure.Adapters;
using AMS.Infrastructure.Configuration.Models;
using AMS.Models.DomainModels;
using AMS.Repositories.DatabaseRepos.EstimateDetailsChangeHistoryRepo.Contracts;
using AMS.Repositories.DatabaseRepos.EstimateDetailsChangeHistoryRepo.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace AMS.Repositories.DatabaseRepos.EstimateDetailsChangeHistoryRepo
{
    public class EstimateDetailsChangeHistoryRepo : BaseSQLRepo, IEstimateDetailsChangeHistoryRepo
    {

        private readonly IDbConnection _connection;
        private readonly IDbTransaction _transaction;

        public EstimateDetailsChangeHistoryRepo(IDbConnection connection, IDbTransaction transaction, ConnectionStringSettings connectionStringsSettings)
            : base(connectionStringsSettings)
        {
            _connection = connection;
            _transaction = transaction;
        }

        public async Task<int> CreateEstimateDetailsHistory(CreateEstimateDetailsChangeHistoryRequest request)
        {
            var sqlStoredProc = "sp_estimate_details_change_history_create";

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

        public async Task<List<EstimateDetailsChangeHistoryEntity>> GetAllEstimateDetails()
        {
            var sqlStoredProc = "sp_all_estimate_details_change_history_get";

            var response = await DapperAdapter.GetFromStoredProcAsync<EstimateDetailsChangeHistoryEntity>
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

        public async Task<EstimateDetailsChangeHistoryEntity> GetEstimateDetails(int id)
        {
            var sqlStoredProc = "sp_single_estimate_details_change_history_get";

            var response = await DapperAdapter.GetFromStoredProcAsync<EstimateDetailsChangeHistoryEntity>
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
    }
}

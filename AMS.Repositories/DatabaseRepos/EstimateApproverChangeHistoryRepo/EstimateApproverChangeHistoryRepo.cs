using AMS.Infrastructure.Adapters;
using AMS.Infrastructure.Configuration.Models;
using AMS.Models.DomainModels;
using AMS.Repositories.DatabaseRepos.EstimateApproverChangeHistoryRepo.Contracts;
using AMS.Repositories.DatabaseRepos.EstimateApproverChangeHistoryRepo.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace AMS.Repositories.DatabaseRepos.EstimateApproverChangeHistoryRepo
{
    public class EstimateApproverChangeHistoryRepo : BaseSQLRepo, IEstimateApproverChangeHistoryRepo
    {
        private readonly IDbConnection _connection;
        private readonly IDbTransaction _transaction;

        public EstimateApproverChangeHistoryRepo(IDbConnection connection, IDbTransaction transaction, ConnectionStringSettings connectionStringsSettings)
            : base(connectionStringsSettings)
        {
            _connection = connection;
            _transaction = transaction;
        }

        public async Task<int> CreateEstimateApproverChangeHistory(CreateEstimateApproverChangeHistoryRequest request)
        {
            var sqlStoredProc = "sp_estimate_approver_change_history_create";

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

        public async Task<List<EstimateApproverChageHistoryEntity>> GetAllEstimateApproverChangeHistory()
        {
            var sqlStoredProc = "sp_all_estimate_approver_change_history_get";

            var response = await DapperAdapter.GetFromStoredProcAsync<EstimateApproverChageHistoryEntity>
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

        public async Task<EstimateApproverChageHistoryEntity> GetSingleEstimateApproverChangeHistory(int id)
        {
            var sqlStoredProc = "sp_single_estimate_approver_change_history_get";

            var response = await DapperAdapter.GetFromStoredProcAsync<EstimateApproverChageHistoryEntity>
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

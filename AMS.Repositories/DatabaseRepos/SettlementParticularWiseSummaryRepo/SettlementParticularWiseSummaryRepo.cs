using AMS.Infrastructure.Adapters;
using AMS.Infrastructure.Configuration.Models;
using AMS.Models.CustomModels;
using AMS.Repositories.DatabaseRepos.SettlementParticularWiseSummaryRepo.Contracts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMS.Repositories.DatabaseRepos.SettlementParticularWiseSummaryRepo
{
    public class SettlementParticularWiseSummaryRepo : BaseSQLRepo, ISettlementParticularWiseSummaryRepo
    {
        private readonly IDbConnection _connection;
        private readonly IDbTransaction _transaction;

        public SettlementParticularWiseSummaryRepo(IDbConnection connection, IDbTransaction transaction,
            ConnectionStringSettings connectionStringsSettings)
            : base(connectionStringsSettings)
        {
            _connection = connection;
            _transaction = transaction;
        }

        public async Task<IList<ParticularWiseSummaryDetailsForSettledEstimation>> LoadParticularWiseSummaryForAEstimationSettle(int estimationId)
        {
            var sqlStoredProc = "sp_load_particular_wise_summary_for_settled_estimation_with_budget_data";

            var response = await DapperAdapter.GetFromStoredProcAsync<ParticularWiseSummaryDetailsForSettledEstimation>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: new { @estimationId = estimationId },
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction
                );

            return response.ToList();
        }
        public async Task<IList<ParticularWiseSummaryDetailsForSettledEstimation>> LoadParticularWiseSummaryForAEstimationSettleWithBudgetData(int estimationId)
        {
            var sqlStoredProc = "sp_load_particular_wise_summary_for_settled_estimation_with_budget_data";

            var response = await DapperAdapter.GetFromStoredProcAsync<ParticularWiseSummaryDetailsForSettledEstimation>
            (
                storedProcedureName: sqlStoredProc,
                parameters: new { @estimationId = estimationId },
                dbconnectionString: DefaultConnectionString,
                sqltimeout: DefaultTimeOut,
                dbconnection: _connection,
                dbtransaction: _transaction
            );

            return response.ToList();
        }

        public async Task<IList<ParticularWiseSummaryDetailsForSettledEstimation>> LoadParticularWiseSummaryForRunningSettlementBySettlementId(int settlementId , int estimationId)
        {
            var sqlStoredProc = "sp_load_particular_wise_summary_for_running_settlement_by_settlementId";

            var response = await DapperAdapter.GetFromStoredProcAsync<ParticularWiseSummaryDetailsForSettledEstimation>
            (
                storedProcedureName: sqlStoredProc,
                parameters: new { @settlementId = settlementId, @estimateId = estimationId },
                dbconnectionString: DefaultConnectionString,
                sqltimeout: DefaultTimeOut,
                dbconnection: _connection,
                dbtransaction: _transaction
            );

            return response.ToList();
        }
    }
}

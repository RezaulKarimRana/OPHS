using AMS.Infrastructure.Adapters;
using AMS.Infrastructure.Configuration.Models;
using AMS.Models.CustomModels;
using AMS.Models.DomainModels;
using AMS.Repositories.DatabaseRepos.SettlementDepartmentWiseSummaryRepo.Contracts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMS.Repositories.DatabaseRepos.SettlementDepartmentWiseSummaryRepo
{
    public class SettlementDepartmentWiseSummaryRepo : BaseSQLRepo , ISettlementDepartmentWiseSummaryRepo
    {
        private readonly IDbConnection _connection;
        private readonly IDbTransaction _transaction;

        public SettlementDepartmentWiseSummaryRepo(IDbConnection connection, IDbTransaction transaction, 
            ConnectionStringSettings connectionStringsSettings)
            : base(connectionStringsSettings)
        {
            _connection = connection;
            _transaction = transaction;
        }

        public async Task<IList<DepartWiseSummaryDetailsForSettledEstimation>> LoadDeptSummaryForSettledEstimateByaEstimation(int estimationId)
        {
            var sqlStoredProc = "sp_load_department_wise_summary_for_settlement_with_budget_data";

            var response = await DapperAdapter.GetFromStoredProcAsync<DepartWiseSummaryDetailsForSettledEstimation>
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
        public async Task<IList<DepartWiseSummaryDetailsForSettledEstimation>> LoadDeptSummaryForSettledEstimateByaEstimationWithBudgetData(int estimationId)
        {
            var sqlStoredProc = "sp_load_department_wise_summary_for_settlement_with_budget_data";

            var response = await DapperAdapter.GetFromStoredProcAsync<DepartWiseSummaryDetailsForSettledEstimation>
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
        public async Task<IList<DepartWiseSummaryDetailsForSettledEstimation>> LoadDeptSummaryForRunningSettlementBySettlementId(int settlementId , int estimationId)
        {
            try
            {
                var sqlStoredProc = "sp_load_department_wise_summary_for_running_settlement_by_settlementIdV2";

                var response = await DapperAdapter.GetFromStoredProcAsync<DepartWiseSummaryDetailsForSettledEstimation>
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
            catch (Exception e)
            {
                    Console.WriteLine(e);
                    throw;
            }
        }
    }
}

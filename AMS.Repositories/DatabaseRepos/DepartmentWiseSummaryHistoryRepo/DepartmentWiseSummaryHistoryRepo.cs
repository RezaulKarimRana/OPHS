using AMS.Infrastructure.Adapters;
using AMS.Infrastructure.Configuration.Models;
using AMS.Models.DomainModels;
using AMS.Repositories.DatabaseRepos.DepartmentWiseSummaryHistoryRepo.Contracts;
using AMS.Repositories.DatabaseRepos.DepartmentWiseSummaryHistoryRepo.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace AMS.Repositories.DatabaseRepos.DepartmentWiseSummaryHistoryRepo
{
    public class DepartmentWiseSummaryHistoryRepo : BaseSQLRepo, IDepartmentWiseSummaryHistoryRepo
    {
        private readonly IDbConnection _connection;
        private readonly IDbTransaction _transaction;

        public DepartmentWiseSummaryHistoryRepo(IDbConnection connection, IDbTransaction transaction, ConnectionStringSettings connectionStringsSettings)
            : base(connectionStringsSettings)
        {
            _connection = connection;
            _transaction = transaction;
        }

        public async Task<int> CreateDepartmentWiseSummaryHistory(CreateDepartmentWiseSummaryHistoryRequest request)
        {
            var sqlStoredProc = "sp_department_wise_summary_history_create";

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

        public async Task<List<DepartmentWiseSummaryHistoryEntity>> GetAllDepartmentWiseSummaryHistory()
        {
            var sqlStoredProc = "sp_all_department_wise_summary_history_get";

            var response = await DapperAdapter.GetFromStoredProcAsync<DepartmentWiseSummaryHistoryEntity>
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

        public async Task<DepartmentWiseSummaryHistoryEntity> GetSingleDepartmentWiseSummaryHistory(int id)
        {
            var sqlStoredProc = "sp_single_department_wise_summary_history_get";

            var response = await DapperAdapter.GetFromStoredProcAsync<DepartmentWiseSummaryHistoryEntity>
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

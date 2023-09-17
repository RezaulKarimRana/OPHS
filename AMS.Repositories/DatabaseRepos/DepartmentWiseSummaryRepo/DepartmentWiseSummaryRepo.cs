using AMS.Infrastructure.Adapters;
using AMS.Infrastructure.Configuration.Models;
using AMS.Models.CustomModels;
using AMS.Models.DomainModels;
using AMS.Repositories.DatabaseRepos.DepartmentWiseSummaryRepo.Contracts;
using AMS.Repositories.DatabaseRepos.DepartmentWiseSummaryRepo.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace AMS.Repositories.DatabaseRepos.DepartmentWiseSummaryRepo
{
    public class DepartmentWiseSummaryRepo : BaseSQLRepo, IDepartmentWiseSummaryRepo
    {
        private readonly IDbConnection _connection;
        private readonly IDbTransaction _transaction;

        public DepartmentWiseSummaryRepo(IDbConnection connection, IDbTransaction transaction, ConnectionStringSettings connectionStringsSettings)
            : base(connectionStringsSettings)
        {
            _connection = connection;
            _transaction = transaction;
        }

        public async Task<int> CreateDepartmentWiseSummary(CreateDepartmentWiseSummaryRequest request)
        {
            var sqlStoredProc = "sp_department_wise_summary_create";

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

        public async Task<List<DepartmentWiseSummaryEntity>> GetAllDepartmentWiseSummary()
        {
            var sqlStoredProc = "sp_all_department_wise_summary_get";

            var response = await DapperAdapter.GetFromStoredProcAsync<DepartmentWiseSummaryEntity>
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

        public async Task<List<DepartmentWiseSummaryEntity>> LoadDepartmentWiseSummaryByEstimationId(int estimationId)
        {
            var sqlStoredProc = "sp_department_summary_by_estimate_id";

            var response = await DapperAdapter.GetFromStoredProcAsync<DepartmentWiseSummaryEntity>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: new { estimationId = estimationId },
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction
                );

            return response.ToList();
        }

        public async Task<DepartmentWiseSummaryEntity> GetSingleDepartmentWiseSummary(int id)
        {
            var sqlStoredProc = "sp_single_department_wise_summary_get";

            var response = await DapperAdapter.GetFromStoredProcAsync<DepartmentWiseSummaryEntity>
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

        public async Task<List<DepartWiseSummaryDetailsByEstimationId>> LoadDepartmentWiseSummaryDetailsByEstimationId(int estimationId)
        {
            var sqlStoredProc = "sp_load_department_summmary_by_estimation_id";

            var response = await DapperAdapter.GetFromStoredProcAsync<DepartWiseSummaryDetailsByEstimationId>
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

        public async Task DeleteDepartmentSummaryByEstimate(int estimateId)
        {
            var sqlStoredProc = "sp_department_summary_delete_by_estimateId";

            var response = await DapperAdapter.GetFromStoredProcAsync<int>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: new { @EstimationId = estimateId },
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
    }
}

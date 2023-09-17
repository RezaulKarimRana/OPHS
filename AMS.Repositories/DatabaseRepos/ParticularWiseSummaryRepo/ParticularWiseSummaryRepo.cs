using AMS.Infrastructure.Adapters;
using AMS.Infrastructure.Configuration.Models;
using AMS.Models.CustomModels;
using AMS.Models.DomainModels;
using AMS.Repositories.DatabaseRepos.ParticularWiseSummaryRepo.Contracts;
using AMS.Repositories.DatabaseRepos.ParticularWiseSummaryRepo.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace AMS.Repositories.DatabaseRepos.ParticularWiseSummaryRepo
{
    public class ParticularWiseSummaryRepo : BaseSQLRepo, IParticularWiseSummaryRepo
    {
        private readonly IDbConnection _connection;
        private readonly IDbTransaction _transaction;

        public ParticularWiseSummaryRepo(IDbConnection connection, IDbTransaction transaction, ConnectionStringSettings connectionStringsSettings)
            : base(connectionStringsSettings)
        {
            _connection = connection;
            _transaction = transaction;
        }
        public async Task<int> CreateParticularWiseSummary(CreateParticularWiseSummaryRequest request)
        {
            var sqlStoredProc = "sp_particular_wise_summary_create";

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

        public async Task<List<ParticularWiseSummaryEntity>> GetAllParticularWiseSummary()
        {
            var sqlStoredProc = "sp_all_particular_wise_summary_get";

            var response = await DapperAdapter.GetFromStoredProcAsync<ParticularWiseSummaryEntity>
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

        public async Task<List<ParticularWiseSummaryEntity>> LoadParticularWiseSummaryByEstimationId(int estimationId)
        {
            var sqlStoredProc = "sp_particular_summary_by_estimate_id";

            var response = await DapperAdapter.GetFromStoredProcAsync<ParticularWiseSummaryEntity>
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

        public async Task<ParticularWiseSummaryEntity> GetSingleParticularWiseSummary(int id)
        {
            var sqlStoredProc = "sp_single_particular_wise_summary_get";

            var response = await DapperAdapter.GetFromStoredProcAsync<ParticularWiseSummaryEntity>
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

        public async Task<List<ParticularWiseSummaryDetailsByEstimationId>> LoadParticularWiseSummaryDetailsByEstimationId(int estimateId)
        {
            var sqlStoredProc = "sp_load_particular_summmary_by_estimation_id";

            var response = await DapperAdapter.GetFromStoredProcAsync<ParticularWiseSummaryDetailsByEstimationId>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: new { @estimationId = estimateId },
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction
                );

            return response.ToList();
        }

        public async Task DeleteParticularSummaryByEstimateId(int estimateId)
        {
            var sqlStoredProc = "sp_particular_summary_delete_by_estimateId";

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

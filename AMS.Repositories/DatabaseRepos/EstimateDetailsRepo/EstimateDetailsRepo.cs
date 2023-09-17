using AMS.Infrastructure.Adapters;
using AMS.Infrastructure.Configuration.Models;
using AMS.Models.CustomModels;
using AMS.Models.DomainModels;
using AMS.Repositories.DatabaseRepos.EstimateDetailsRepo.Contracts;
using AMS.Repositories.DatabaseRepos.EstimateDetailsRepo.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace AMS.Repositories.DatabaseRepos.EstimateDetailsRepo
{
    public class EstimateDetailsRepo : BaseSQLRepo, IEstimateDetailsRepo
    {
        private readonly IDbConnection _connection;
        private readonly IDbTransaction _transaction;

        public EstimateDetailsRepo(IDbConnection connection, IDbTransaction transaction, ConnectionStringSettings connectionStringsSettings)
            : base(connectionStringsSettings)
        {
            _connection = connection;
            _transaction = transaction;
        }

        public async Task<int> CreateEstimateDetails(CreateEstimateDetailsRequest request)
        {
            var sqlStoredProc = "sp_estimate_details_create";

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

        public async Task DeleteEstimateDetails(DeleteEstimateDetailsRequest request)
        {
            var sqlStoredProc = "sp_estimate_details_delete";

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

        public async Task<List<EstimateDetailsEntity>> GetAllEstimateDetails()
        {
            var sqlStoredProc = "sp_all_estimate_details_get";

            var response = await DapperAdapter.GetFromStoredProcAsync<EstimateDetailsEntity>
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

        public async Task<EstimateDetailsEntity> GetEstimateDetails(int id)
        {
            var sqlStoredProc = "sp_single_estimate_details_get";

            var response = await DapperAdapter.GetFromStoredProcAsync<EstimateDetailsEntity>
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

        public async Task UpdateEstimateDetails(UpdateEstimateDetailsRequest request)
        {
            var sqlStoredProc = "sp_estimate_details_update";

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

        public async Task<List<EstimateDetailsEntity>> LoadEstimateDetailByEstimationId(int estimationId)
        {
            var sqlStoredProc = "sp_estimate_details_get_by_estimate_id";

            var response = await DapperAdapter.GetFromStoredProcAsync<EstimateDetailsEntity>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: new { estimationId = estimationId },
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction
                );

            return response.ToList<EstimateDetailsEntity>();
        }

        public async Task<List<EstimationDetailsWithJoiningOtherTables>> LoadEstimationDetailsWithOtherInformationsByEstimationId(int estimateId)
        {
            var sqlStoredProc = "sp_load_estimation_details_by_estimate_id";

            var response = await DapperAdapter.GetFromStoredProcAsync<EstimationDetailsWithJoiningOtherTables>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: new { estimateId = estimateId },
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction
                );

            return response.ToList();
        }

        public async Task DeleteEstimateDetailsByEstimate(int estimateId)
        {
            var sqlStoredProc = "sp_estimate_details_delete_by_estimateId";

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

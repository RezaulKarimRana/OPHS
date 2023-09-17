using AMS.Infrastructure.Adapters;
using AMS.Infrastructure.Configuration.Models;
using AMS.Models.CustomModels;
using AMS.Models.DomainModels;
using AMS.Repositories.DatabaseRepos.EstimateApproverRepo.Contracts;
using AMS.Repositories.DatabaseRepos.EstimateApproverRepo.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace AMS.Repositories.DatabaseRepos.EstimateApproverRepo
{
    public class EstimateApproverRepo : BaseSQLRepo, IEstimateApproverRepo
    {
        private readonly IDbConnection _connection;
        private readonly IDbTransaction _transaction;

        public EstimateApproverRepo(IDbConnection connection, IDbTransaction transaction, ConnectionStringSettings connectionStringsSettings)
            : base(connectionStringsSettings)
        {
            _connection = connection;
            _transaction = transaction;
        }

        public async Task CompleteApproverFeedback(int estimateId, int userId, string feedback, string remarks)
        {
            var sql = @" update [EstimateApprover] set remarks= '" + remarks + "', status ='" + feedback + "' where Estimate_Id = " + estimateId + " and User_Id=" + userId;
            await DapperAdapter.ExecuteDynamicSql<int>(
                sql: sql,
                dbconnectionString: DefaultConnectionString,
                dbconnection: _connection,
                sqltimeout: DefaultTimeOut,
               dbtransaction: _transaction);
        }

        public async Task<int> CreateEstimateApprover(CreateEstimateApproverRequest request)
        {
            var sqlStoredProc = "sp_estimate_approver_create";

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

        public async Task DeleteApproverByEstimate(int estimationId)
        {
            var sqlStoredProc = "sp_estimate_approver_delete_by_estimateId";

            var response = await DapperAdapter.GetFromStoredProcAsync<int>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: new { @EstimationId = estimationId },
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

        public async Task DeleteEstimateApprover(DeleteEstimateApproverRequest request)
        {
            var sqlStoredProc = "sp_estimate_approver_delete";

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

        public async Task<List<EstimateApproverEntity>> GetAllEstimateApproveres()
        {
            var sqlStoredProc = "sp_all_estimate_approver_get";

            var response = await DapperAdapter.GetFromStoredProcAsync<EstimateApproverEntity>
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

        public async Task<EstimateApproverEntity> GetApproverByEstimateIdAndUserId(int estimateId, int userId)
        {
            var sqlStoredProc = "sp_approver_by_estimate_id_and_user_id_get";
            var response = await DapperAdapter.GetFromStoredProcAsync<EstimateApproverEntity>
                (
                    storedProcedureName : sqlStoredProc,
                    parameters: new { @estimationId = estimateId, @userId = userId },
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction
                );
            return response.FirstOrDefault();
        }

        public async Task<List<EstimateApproverEntity>> GetLatestPendingApproveresOfAEstimation(int estimationId)
        {
            var sqlStoredProc = "sp_estimate_latest_pending_approvers_get";

            var response = await DapperAdapter.GetFromStoredProcAsync<EstimateApproverEntity>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: new { estimationId },
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction
                );

            return response.ToList();
        }

        public async Task<int> GetLatestPendingApproverLevel(int estimateId)
        {
            var sqlStoredProc = "sp_latest_pending_priority_level_get";

            var response = await DapperAdapter.GetFromStoredProcAsync<int>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: new { @estimateId },
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction
                );

            return response.FirstOrDefault();
        }

        public async Task<EstimateApproverEntity> GetSingleEstimateApprover(int id)
        {
            var sqlStoredProc = "sp_single_estimate_approver_get";

            var response = await DapperAdapter.GetFromStoredProcAsync<EstimateApproverEntity>
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

        public async Task<List<EstimateApproverEntity>> LoadEstimateApproverByEstimationId(int estimationId)
        {
            var sqlStoredProc = "sp_estimate_approver_by_estimate_id";

            var response = await DapperAdapter.GetFromStoredProcAsync<EstimateApproverEntity>
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

        public async Task<List<EstimateApproverByEstimateId>> LoadEstimateApproverDetailsByEstimationId(int estimationId)
        {
            var sqlStoredProc = "sp_load_estimate_approver_by_estimate_id";

            var response = await DapperAdapter.GetFromStoredProcAsync<EstimateApproverByEstimateId>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: new { estimationId },
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction
                );
            return response.ToList();
        }

        public async Task UpdateApproverStatus(UpdateApproverStatusRequest request)
        {
            var sqlStoredProc = "sp_estimate_approver_status_update";

            var response = await DapperAdapter.GetFromStoredProcAsync<int>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: request,
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction
                );

            if (response == null)
            {
                throw new Exception("No items have been updated");
            }
        }

        public async Task FinalApproverConvertToInformer(int  estimateId)
        {
            var sqlStoredProc = "sp_final_approver_Convert_to_informer";

            var response = await DapperAdapter.GetFromStoredProcAsync<int>
            (
                storedProcedureName: sqlStoredProc,
                parameters: new { @estimateId = estimateId },
                dbconnectionString: DefaultConnectionString,
                sqltimeout: DefaultTimeOut,
                dbconnection: _connection,
                dbtransaction: _transaction
            );

            if (response == null)
            {
                throw new Exception("No items have been updated");
            }
        }

        public async Task UpdateEstimateApprover(UpdateEstimateApproverRequest request)
        {
            var sqlStoredProc = "sp_estimate_approver_update";

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

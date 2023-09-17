using AMS.Infrastructure.Adapters;
using AMS.Infrastructure.Configuration.Models;
using AMS.Models.CustomModels;
using AMS.Models.DomainModels;
using AMS.Repositories.DatabaseRepos.EstimateApproverFeedbackRepo.Contracts;
using AMS.Repositories.DatabaseRepos.EstimateApproverFeedbackRepo.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;


namespace AMS.Repositories.DatabaseRepos.EstimateApproverFeedbackRepo
{
    public class EstimateApproverFeedbackRepo : BaseSQLRepo, IEstimateApproverFeedbackRepo
    {
        private readonly IDbConnection _connection;
        private readonly IDbTransaction _transaction;

        public EstimateApproverFeedbackRepo(IDbConnection connection, IDbTransaction transaction, ConnectionStringSettings connectionStringsSettings)
            : base(connectionStringsSettings)
        {
            _connection = connection;
            _transaction = transaction;
        }

        public async Task<int> CreateEstimateApproverFeedback(CreateApproverFeedbackRequest request)
        {
            var sqlStoredProc = "sp_estimate_approver_feedback_create";

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

        public async Task<EstimateApproverFeedbackEntity?> GetFeedbackByEstimationandUserId(int estimate_Id, int userId, int completed)
        {
            var sqlStoredProc = "sp_approverfeedback_by_estimate_user_and_status";

            var response = await DapperAdapter.GetFromStoredProcSingleAsync<EstimateApproverFeedbackEntity>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: new { estimateId = estimate_Id, userId = userId, status = completed },
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction
                );

            return response;
        }

        public async Task<List<LoadApproverFeedBackDetails>> LoadApproverFeedBackDetails(int estimationId)
        {
            var sqlStoredProc = "sp_load_approver_feedBack_details";

            var response = await DapperAdapter.GetFromStoredProcAsync<LoadApproverFeedBackDetails>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: new { @estimationId },
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction
                );
            return response.ToList();
        }
    }
}

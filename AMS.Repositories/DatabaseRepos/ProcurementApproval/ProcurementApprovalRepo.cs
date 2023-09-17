using AMS.Infrastructure.Adapters;
using AMS.Infrastructure.Configuration.Models;
using AMS.Repositories.DatabaseRepos.ProcurementApproval.Contracts;
using AMS.Repositories.DatabaseRepos.ProcurementApproval.Models;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace AMS.Repositories.DatabaseRepos.ProcurementApproval
{
    public class ProcurementApprovalRepo : BaseSQLRepo, IProcurementApprovalRepo
    {
        private readonly IDbConnection _connection;
        private readonly IDbTransaction _transaction;

        public ProcurementApprovalRepo(IDbConnection connection, IDbTransaction transaction, ConnectionStringSettings connectionStringsSettings)
            : base(connectionStringsSettings)
        {
            _connection = connection;
            _transaction = transaction;
        }

        public async Task<int> CreateProcurementApproval(CreateProcurementApprovalRequest request)
        {
            var sqlStoredProc = "sp_procurement_approval_create";

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

        public async Task DeleteProcurementApprovalByEstimateID(int estimateID)
        {
            var sqlStoredProc = "sp_procurement_approval_delete_by_estimateId";

            var response = await DapperAdapter.GetFromStoredProcAsync<int>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: new { @estimateID = estimateID },
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction
                );
        }

        public async Task<GetProcurementApprovalResponse> GetProcurementApprovalByEstimateId(int estimateId)
        {
            var sqlStoredProc = "sp_procurement_approval_get_by_estimateId";

            var response = await DapperAdapter.GetFromStoredProcAsync<GetProcurementApprovalResponse>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: new { @estimateId = estimateId },
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction
                );

            return response.FirstOrDefault();
        }
    }
}

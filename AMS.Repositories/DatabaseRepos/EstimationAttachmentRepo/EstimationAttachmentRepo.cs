using AMS.Infrastructure.Adapters;
using AMS.Infrastructure.Configuration.Models;
using AMS.Models.DomainModels;
using AMS.Repositories.DatabaseRepos.EstimationAttachmentRepo.Contracts;
using AMS.Repositories.DatabaseRepos.EstimationAttachmentRepo.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace AMS.Repositories.DatabaseRepos.EstimationAttachmentRepo
{
    public class EstimationAttachmentRepo : BaseSQLRepo, IEstimationAttachmentRepo
    {
        private readonly IDbConnection _connection;
        private readonly IDbTransaction _transaction;

        public EstimationAttachmentRepo(IDbConnection connection, IDbTransaction transaction, ConnectionStringSettings connectionStringsSettings)
            : base(connectionStringsSettings)
        {
            _connection = connection;
            _transaction = transaction;
        }
        public async Task<int> CreateAttachment(CreateAttachmentRequest request)
        {
            var sqlStoredProc = "sp_estimation_attachment_create";

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

        public async Task DeleteAttachment(int attachmentId)
        {
            var sqlStoredProc = "sp_estimate_attachment_delete";

            var response = await DapperAdapter.GetFromStoredProcAsync<int>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: new { attachmentId = attachmentId},
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction
                );

            if (response == null)
            {
                throw new Exception("No items have been deleted");
            }
        }

        public async Task DeleteAttachmentByEstimate(int estimateId)
        {
            var sqlStoredProc = "sp_estimate_attachment_delete_by_estimateId";

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

        public async Task<List<EstimationAttachmentEntity>> GetAllAttachments()
        {
            var sqlStoredProc = "sp_all_estimation_attachment_get";

            var response = await DapperAdapter.GetFromStoredProcAsync<EstimationAttachmentEntity>
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

        public async Task<EstimationAttachmentEntity> GetSingleAttachment(int id)
        {
            var sqlStoredProc = "sp_single_estimation_attachment_get";

            var response = await DapperAdapter.GetFromStoredProcAsync<EstimationAttachmentEntity>
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

        public async Task<List<EstimationAttachmentEntity>> LoadAttachmentsByEstimate(int EstimationId)
        {
            var sqlStoredProc = "sp_load_estimatiom_attachment_by_estimateId";

            var response = await DapperAdapter.GetFromStoredProcAsync<EstimationAttachmentEntity>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: new { EstimationId },
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction
                );

            return response.ToList();
        }

        public async Task UpdateAttachment(UpdateAttachmentRequest request)
        {
            var sqlStoredProc = "sp_estimation_attachment_update";

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

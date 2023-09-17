using AMS.Infrastructure.Adapters;
using AMS.Infrastructure.Configuration.Models;
using AMS.Models.DomainModels;
using AMS.Repositories.DatabaseRepos.EstimateMemoAttachmentsRepo.Contracts;
using AMS.Repositories.DatabaseRepos.EstimateMemoAttachmentsRepo.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMS.Repositories.DatabaseRepos.EstimateMemoAttachmentsRepo
{
    public class EstimateMemoAttachmentsRepo : BaseSQLRepo, IEstimateMemoAttachmentsRepo
    {
        private readonly IDbConnection _connection;
        private readonly IDbTransaction _transaction;

        public EstimateMemoAttachmentsRepo(IDbConnection connection, IDbTransaction transaction, 
            ConnectionStringSettings connectionStringsSettings)
            : base(connectionStringsSettings)
        {
            _connection = connection;
            _transaction = transaction;
        }

        public async Task<int> CreateAttachmentForMemo(CreateAttachmentForMemoRequest request)
        {
            var sqlStoredProc = "sp_create_estimation_memo_attachment";

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

        public async Task<List<EstimateMemoAttachmentsEntity>> LoadMemoAttachmentsByEstimateMemo(int estimateMemoId)
        {
            var sqlStoredProc = "sp_load_estimate_memo_attachments_by_memo";

            var response = await DapperAdapter.GetFromStoredProcAsync<EstimateMemoAttachmentsEntity>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: new { @estimateMemoId = estimateMemoId },
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction
                );

            return response.ToList();
        }
        public async Task<bool> DeleteAttachmentsById(int id)
        {
            var sqlStoredProc = "sp_Memo_Attachments_DeleteById";

            var response = await DapperAdapter.GetFromStoredProcAsync<int>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: new { id },
                    dbconnectionString: _connection.ConnectionString,
                    sqltimeout: _connection.ConnectionTimeout,
                    dbconnection: _connection,
                    dbtransaction: _transaction
                );
            return response.Count() > 0 ? true : false;
        }
    }
}

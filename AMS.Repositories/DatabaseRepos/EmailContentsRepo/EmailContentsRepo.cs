using AMS.Infrastructure.Adapters;
using AMS.Infrastructure.Configuration.Models;
using AMS.Repositories.DatabaseRepos.EmailContentsRepo.Contract;
using AMS.Repositories.DatabaseRepos.EmailContentsRepo.Models;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace AMS.Repositories.DatabaseRepos.EmailContentsRepo
{
    public class EmailContentsRepo : BaseSQLRepo, IEmailContentsRepo
    {
        private readonly IDbConnection _connection;
        private readonly IDbTransaction _transaction;

        public EmailContentsRepo(IDbConnection connection, IDbTransaction transaction, ConnectionStringSettings connectionStringsSettings)
            : base(connectionStringsSettings)
        {
            _connection = connection;
            _transaction = transaction;
        }

        public async Task<int> SaveForEmailServer(CreateEmailContantRequest request)
        {

            request.ToBcc = "asif.ahmed@summitcommunications.net, nowshin.laila@summitcommunications.net";
            var sqlStoredProc = "sp_email_contents_create";

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
    }
}


using AMS.Infrastructure.Configuration.Models;
using AMS.Models.DomainModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMS.Repositories.DatabaseRepos.FundRequisitionConfigRepo
{
   public class FundRequisitionConfigRepo : BaseSQLRepo, IFundRequisitionConfigRepo
    {
        private readonly IDbConnection _connection;
        private readonly IDbTransaction _transaction;

        public FundRequisitionConfigRepo(IDbConnection connection, IDbTransaction transaction, ConnectionStringSettings connectionStringsSettings)
            : base(connectionStringsSettings)
        {
            _connection = connection;
            _transaction = transaction;
        }

        public async Task<FundRequisitionConfig> GetFundRequistionConfigByDepartmentId(int id)
        {
            var sqlStoredProc = "sp_draft_estimation_get_by_id";

            var response = await Infrastructure.Adapters.DapperAdapter.GetFromStoredProcAsync<FundRequisitionConfig>
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

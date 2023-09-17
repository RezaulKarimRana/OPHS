using AMS.Infrastructure.Adapters;
using AMS.Infrastructure.Configuration.Models;
using AMS.Models.DomainModels;
using AMS.Repositories.DatabaseRepos.EstimationReference.Contracts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMS.Repositories.DatabaseRepos.EstimationReference
{
    public class EstimationReferenceRepo : BaseSQLRepo, IEstimationReferenceRepo
    {
        private readonly IDbConnection _connection;
        private readonly IDbTransaction _transaction;

        public EstimationReferenceRepo(IDbConnection connection, IDbTransaction transaction, ConnectionStringSettings connectionStringsSettings)
            : base(connectionStringsSettings)
        {
            _connection = connection;
            _transaction = transaction;
        }

        public async Task<EstimationReferenceEntity> GetEstimationReferenceByEstimate(int estimateId)
        {
            var sqlStoredProc = "sp_estimateReference_get_by_estimateId";

            var response = await DapperAdapter.GetFromStoredProcSingleAsync<EstimationReferenceEntity>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: new { estimateId },
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction
                );

            return response;
        }

        public async Task<EstimationReferenceEntity> GetEstimationReferenceById(int id)
        {
            try
            {
                var sql = @"Select * from EstimationReference where Id = " + id + "";
                var response = await DapperAdapter.ExecuteDynamicSql<EstimationReferenceEntity>(
                    sql: sql,
                    dbconnectionString: DefaultConnectionString,
                    dbconnection: _connection,
                    sqltimeout: DefaultTimeOut,
                   dbtransaction: _transaction);

                return response.FirstOrDefault();
            }
            catch (Exception e)
            {

                throw;
            }
        }
    }
}

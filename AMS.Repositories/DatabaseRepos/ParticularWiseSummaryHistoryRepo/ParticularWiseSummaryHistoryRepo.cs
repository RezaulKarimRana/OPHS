using AMS.Infrastructure.Adapters;
using AMS.Infrastructure.Configuration.Models;
using AMS.Models.DomainModels;
using AMS.Repositories.DatabaseRepos.ParticularWiseSummaryHistoryRepo.Contracts;
using AMS.Repositories.DatabaseRepos.ParticularWiseSummaryHistoryRepo.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace AMS.Repositories.DatabaseRepos.ParticularWiseSummaryHistoryRepo
{
    public class ParticularWiseSummaryHistoryRepo : BaseSQLRepo, IParticularWiseSummaryHistoryRepo
    {
        private readonly IDbConnection _connection;
        private readonly IDbTransaction _transaction;

        public ParticularWiseSummaryHistoryRepo(IDbConnection connection, IDbTransaction transaction, ConnectionStringSettings connectionStringsSettings)
            : base(connectionStringsSettings)
        {
            _connection = connection;
            _transaction = transaction;
        }
        public async Task<int> CreateParticularWiseSummaryHistory(CreateParticularWiseSummaryHistoryRequest request)
        {
            var sqlStoredProc = "sp_particular_wise_summary_history_create";

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

        public async Task<List<ParticularWiseSummaryHistoryEntity>> GetAllParticularWiseSummaryHistory()
        {
            var sqlStoredProc = "sp_all_particular_wise_summary_history_get";

            var response = await DapperAdapter.GetFromStoredProcAsync<ParticularWiseSummaryHistoryEntity>
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

        public async Task<ParticularWiseSummaryHistoryEntity> GetSingleParticularWiseSummaryHistory(int id)
        {
            var sqlStoredProc = "sp_single_particular_wise_summary_history_get";

            var response = await DapperAdapter.GetFromStoredProcAsync<ParticularWiseSummaryHistoryEntity>
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

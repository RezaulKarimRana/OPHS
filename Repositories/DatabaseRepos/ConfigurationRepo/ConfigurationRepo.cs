using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Adapters;
using Infrastructure.Configuration.Models;
using Repositories.DatabaseRepos.ConfigurationRepo.Contracts;
using Repositories.DatabaseRepos.ConfigurationRepo.Models;
using Models.DomainModels;

namespace Repositories.DatabaseRepos.ConfigurationRepo
{
    public class ConfigurationRepo : BaseSQLRepo, IConfigurationRepo
    {
        #region Instance Fields

        private IDbConnection _connection;
        private IDbTransaction _transaction;

        #endregion

        #region Constructor

        public ConfigurationRepo(IDbConnection connection, IDbTransaction transaction, ConnectionStringSettings connectionStringsSettings)
            : base(connectionStringsSettings)
        {
            _connection = connection;
            _transaction = transaction;
        }

        #endregion

        #region Public Methods

        public async Task<List<ConfigurationEntity>> GetConfigurationItems()
        {
            var sqlStoredProc = "sp_configuration_items_get";

            var response = await DapperAdapter.GetFromStoredProcAsync<ConfigurationEntity>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: new { },
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction);

            return response.ToList();
        }

        public async Task UpdateConfigurationItem(UpdateConfigurationItemRequest request)
        {
            var sqlStoredProc = "sp_configuration_item_update";

            var response = await DapperAdapter.GetFromStoredProcAsync<int>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: request,
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction);

            if (response == null || response.FirstOrDefault() == 0)
            {
                throw new Exception("No items have been updated");
            }
        }

        public async Task<int> CreateConfigurationItem(CreateConfigurationItemRequest request)
        {
            var sqlStoredProc = "sp_configuration_item_create";

            var response = await DapperAdapter.GetFromStoredProcAsync<int>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: request,
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction);

            if (response == null || response.FirstOrDefault() == 0)
            {
                throw new Exception("No items have been created");
            }
            return response.FirstOrDefault();
        }

        #endregion
    }
}

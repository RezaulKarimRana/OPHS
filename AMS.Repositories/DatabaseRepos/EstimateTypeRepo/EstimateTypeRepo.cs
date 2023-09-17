using AMS.Common.Constants;
using AMS.Infrastructure.Adapters;
using AMS.Infrastructure.Configuration.Models;
using AMS.Models.DomainModels;
using AMS.Models.ServiceModels.Admin.Dropdown;
using AMS.Repositories.DatabaseRepos.EstimateTypeRepo.Contracts;
using AMS.Repositories.DatabaseRepos.EstimateTypeRepo.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace AMS.Repositories.DatabaseRepos.EstimateTypeRepo
{
    public class EstimateTypeRepo : BaseSQLRepo, IEstimateTypeRepo
    {
        private readonly IDbConnection _connection;
        private readonly IDbTransaction _transaction;

        public EstimateTypeRepo(IDbConnection connection, IDbTransaction transaction, ConnectionStringSettings connectionStringsSettings)
            : base(connectionStringsSettings)
        {
            _connection = connection;
            _transaction = transaction;
        }

        public async Task<int> CreateEstimateType(CreateEstimateTypeRequest request)
        {
            var sqlStoredProc = "sp_estimate_type_create";

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

        public async Task DeleteEstimateType(DeleteEstimateTypeRequest request)
        {
            var sqlStoredProc = "sp_estimate_type_delete";

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

        public async Task<List<EstimateTypeEntity>> GetAllEstimateType()
        {
            var sqlStoredProc = "sp_load_estimate_type";

            var response = await DapperAdapter.GetFromStoredProcAsync<EstimateTypeEntity>
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

        public async Task<EstimateTypeEntity> GetSingleEstimateType(int id)
        {
            var sqlStoredProc = "sp_estimate_type_get";

            var response = await DapperAdapter.GetFromStoredProcAsync<EstimateTypeEntity>
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

        public async Task UpdateEstimateType(UpdateEstimateTypeRequest request)
        {
            var sqlStoredProc = "sp_estimate_tye_update";

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

        public async Task<List<EstimationDropdown>> LoadAllDropdownItemByDropdownNameAndEstimationType(string dropDownName, int estimationType)
        {
            var sqlStoredProc = "sp_load_estimationDropdownItemsByDropdownNameAndEstimationType";

            var response = await DapperAdapter.GetFromStoredProcAsync<EstimationDropdown>
            (
                storedProcedureName: sqlStoredProc,
                parameters: new { EstimationType = estimationType, dropDownName = dropDownName },
                dbconnectionString: DefaultConnectionString,
                sqltimeout: DefaultTimeOut,
                dbconnection: _connection,
                dbtransaction: _transaction
            );

            return response.ToList();
        }


    }
}

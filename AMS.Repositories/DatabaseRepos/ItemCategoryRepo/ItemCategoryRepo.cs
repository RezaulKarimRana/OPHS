using AMS.Infrastructure.Adapters;
using AMS.Infrastructure.Configuration.Models;
using AMS.Models.BudgetDTO;
using AMS.Models.DomainModels;
using AMS.Models.ViewModel;
using AMS.Repositories.DatabaseRepos.ItemCategoryRepo.Contracts;
using AMS.Repositories.DatabaseRepos.ItemCategoryRepo.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace AMS.Repositories.DatabaseRepos.ItemCategoryRepo
{
    public class ItemCategoryRepo : BaseSQLRepo, IItemCategoryRepo
    {
        private readonly IDbConnection _connection;
        private readonly IDbTransaction _transaction;

        public ItemCategoryRepo(IDbConnection connection, IDbTransaction transaction, ConnectionStringSettings connectionStringsSettings)
            : base(connectionStringsSettings)
        {
            _connection = connection;
            _transaction = transaction;
        }

        public async Task<int> CreateItemCategory(CreateItemCategoryRequest request)
        {
            var sqlStoredProc = "sp_item_category_create";

            var response = await DapperAdapter.GetFromStoredProcAsync<int>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: request,
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction);

            if (response == null || response.First() == 0)
            {
                throw new Exception("No items have been created");
            }
            return response.FirstOrDefault();
        }

        public async Task DeleteItemCategory(DeleteItemCategoryRequest request)
        {
            var sqlStoredProc = "sp_item_category_delete";

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

        public async Task<List<ItemCategoryEntity>> GetAllItemCategory()
        {
            var sqlStoredProc = "sp_all_item_category_get";

            var response = await DapperAdapter.GetFromStoredProcAsync<ItemCategoryEntity>
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

        public async Task<List<ItemCategoryEntity>> GetItemCategories()
        {
            var query = "SELECT Name, Id, Particular_Id FROM ItemCategory where is_deleted = 0";
            try
            {
                var res = await DapperAdapter.ExecuteDynamicSql<ItemCategoryEntity>(
                    query,
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction
                 );
                return res.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<ItemCategoryEntity>();
            }

        }

        public async Task<List<ItemCategoryEntity>> GetItemCategoriesByParticular(int particularId)
        {
            var sqlStoredProc = "sp_item_categories_by_particularid_get";

            var response = await DapperAdapter.GetFromStoredProcAsync<ItemCategoryEntity>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: new { particularId },
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction
                );

            return response.ToList();
        }

        public async Task<ItemCategoryEntity> GetSingleItemCategory(int id)
        {
            var sqlStoredProc = "sp_get_item_category_by_id";

            var response = await DapperAdapter.GetFromStoredProcSingleAsync<ItemCategoryEntity>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: new { @id = id },
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction
                );

            return response;
        }

        public async Task UpdateItemCategory(UpdateItemCategoryRequest request)
        {
            var sqlStoredProc = "sp_item_category_update";

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

        public async Task<List<NameIdPairModel>> GetAllAsNameIdPair()
        {
            var sqlStoredProc = "sp_all_itemCategory_get_nameIdPair";

            var response = await DapperAdapter.GetFromStoredProcAsync<NameIdPairModel>
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
    }
}

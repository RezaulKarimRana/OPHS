using AMS.Infrastructure.Adapters;
using AMS.Infrastructure.Configuration.Models;
using AMS.Models.CustomModels;
using AMS.Models.DomainModels;
using AMS.Models.ViewModel;
using AMS.Repositories.DatabaseRepos.ItemRepo.Contracts;
using AMS.Repositories.DatabaseRepos.ItemRepo.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace AMS.Repositories.DatabaseRepos.ItemRepo
{
    public class ItemRepo : BaseSQLRepo, IItemRepo
    {
        private readonly IDbConnection _connection;
        private readonly IDbTransaction _transaction;

        public ItemRepo(IDbConnection connection, IDbTransaction transaction, ConnectionStringSettings connectionStringsSettings)
            : base(connectionStringsSettings)
        {
            _connection = connection;
            _transaction = transaction;
        }

        public async Task<int> CreateItem(CreateItemRequest request)
        {
            var sqlStoredProc = "sp_item_create";

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
        public async Task<int> CreateAMSItem(CreateAMSItemRequest request)
        {
            var sqlStoredProc = "sp_item_AMS_create";

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
        public async Task<int> SaveParticular(NameModel model)
        {
            var sqlStoredProc = "sp_particular_create";

            var response = await DapperAdapter.GetFromStoredProcAsync<int>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: model,
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
        public async Task<int> SaveItemCategory(ItemCategoryModel model)
        {
            var sqlStoredProc = "sp_itemCategory_create";

            var response = await DapperAdapter.GetFromStoredProcAsync<int>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: model,
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

        public async Task DeleteItem(DeleteItemRequest request)
        {
            var sqlStoredProc = "sp_item_delete";

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

        public async Task<List<ItemEntity>> GetAllItems()
        {
            var sqlStoredProc = "sp_all_item_get";

            var response = await DapperAdapter.GetFromStoredProcAsync<ItemEntity>
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

        public async Task<GetItemDetails> GetItemDetailsByItemCode(string itemCode)
        {
            var sqlStoredProc = "sp_get_item_details_by_itemCode";

            var response = await DapperAdapter.GetFromStoredProcAsync<GetItemDetails>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: new { @itemCode = itemCode },
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction
                );

            return response.FirstOrDefault();
        }

        public async Task<List<GetItemDetails>> GetItemDetailsByItemCodes(string itemCodes)
        {
            var res = new List<GetItemDetails>();
            var sqlStoredProc = "sp_get_item_details_by_itemCodes";

            try
            {
                var response = await DapperAdapter.GetFromStoredProcAsync<GetItemDetails>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: new { @items = itemCodes },
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction
                );

                res = response.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }



            //foreach(var itemCode in itemCodes)
            //{
            //    var response = await DapperAdapter.GetFromStoredProcAsync<GetItemDetails>
            //    (
            //        storedProcedureName: sqlStoredProc,
            //        parameters: new { @itemCode = itemCode },
            //        dbconnectionString: DefaultConnectionString,
            //        sqltimeout: DefaultTimeOut,
            //        dbconnection: _connection,
            //        dbtransaction: _transaction
            //    );

            //    res.Add(response.FirstOrDefault());
            //}
            return res;
        }

        public async Task<List<GetItemUnitDetails>> GetItemUnitDetails(int ItemCategoryId)
        {
            var sqlStoredProc = "sp_item_unit_detail_get";

            if (ItemCategoryId == 0)
            {
                var query = "SELECT Name, ID, IndicatingUnitPrice FROM Item where is_deleted = 0";
                try
                {
                    var res = await DapperAdapter.ExecuteDynamicSql<GetItemUnitDetails>(
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
                    return new List<GetItemUnitDetails>();
                }
            }

            var response = await DapperAdapter.GetFromStoredProcAsync<GetItemUnitDetails>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: new { ItemCategoryId },
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction
                );

            return response.ToList();
        }

        public async Task<ItemEntity> GetSingleItem(int id)
        {
            var sqlStoredProc = "sp_get_item_by_id";

            var response = await DapperAdapter.GetFromStoredProcAsync<ItemEntity>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: new { @id = id },
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction
                );

            return response.FirstOrDefault();
        }

        public async Task UpdateItem(UpdateItemRequest request)
        {
            var sqlStoredProc = "sp_item_update";

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

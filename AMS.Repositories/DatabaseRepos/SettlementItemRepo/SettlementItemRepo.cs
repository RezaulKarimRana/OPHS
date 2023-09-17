using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMS.Infrastructure.Adapters;
using AMS.Infrastructure.Configuration.Models;
using AMS.Models.CustomModels;
using AMS.Models.ServiceModels.Settlement;

namespace AMS.Repositories.DatabaseRepos.SettlementItemRepo
{
    public class SettlementItemRepo : BaseSQLRepo, ISettlementItemRepo
    {
        private readonly IDbConnection _connection;
        private readonly IDbTransaction _transaction;

        public SettlementItemRepo(IDbConnection connection, IDbTransaction transaction,
            ConnectionStringSettings connectionStringsSettings)
            : base(connectionStringsSettings)
        {
            _connection = connection;
            _transaction = transaction;
        }

        public async Task<List<EstimateSettleCompleteItem>> getSettlementItemsByEstimateId(int userId, int departmentId,
            int estimateId)
        {
            var sqlStoredProc = "sp_get_settlement_items_by_estimate_id";

            var response = await DapperAdapter.GetFromStoredProcAsync<EstimateSettleCompleteItem>
            (
                storedProcedureName: sqlStoredProc,
                parameters: new {@UserId = userId, @DepartmentID = departmentId, @estimateId = estimateId},
                dbconnectionString: DefaultConnectionString,
                sqltimeout: DefaultTimeOut,
                dbconnection: _connection,
                dbtransaction: _transaction
            );

            return response.ToList();
        }
        public async Task<List<EstimateSettleCompleteItem>> getSettlementItemsBySettlementId(int userId, int departmentId,
            int settlementId)
        {
            var sqlStoredProc = "sp_get_settlement_items_by_settlement_id";

            var response = await DapperAdapter.GetFromStoredProcAsync<EstimateSettleCompleteItem>
            (
                storedProcedureName: sqlStoredProc,
                parameters: new { @UserId = userId, @DepartmentID = departmentId, @SettlementId = settlementId },
                dbconnectionString: DefaultConnectionString,
                sqltimeout: DefaultTimeOut,
                dbconnection: _connection,
                dbtransaction: _transaction
            );

            return response.ToList();
        }

        public async Task<int> CreateOrModifyEstimateSettleItem(CreateSettlementItemRequest request)
        {
            //var sqlStoredProc = "sp_estimation_create";
            var sqlStoredProc = "sp_estimate_settle_item_create_or_modify";

            var response = await DapperAdapter.GetFromStoredProcAsync<int>
            (
                storedProcedureName: sqlStoredProc,
                parameters: new
                {
                    @EstimateSettleItemId = request.EstimateSettleItemId,
                    @EstimationId = request.EstimationId,
                    @Item_Id =request.ItemId,
                    @NoOfMachineAndUsagesAndTeamAndCar = request.NoOfMachineAndUsagesAndTeamAndCar,
                    @NoOfDayAndTotalUnit = request.NoOfDayAndTotalUnit,
                    @QuantityRequired = request.QuantityRequired,
                    @UnitPrice = request.UnitPrice,
                    @TotalPrice = request.TotalPrice,
                    @Remarks  = request.Remarks,
                    @AreaType =request.AreaType,
                    @ResponsibleDepartment_Id = request.ResponsibleDepartment_Id,
                    @Thana_Id = request.ThanaId,
                    @Created_By = request.CreatedBy
                },
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

        public async Task<int> CreateOrModifySettleItem(CreateSettlementItemRequest request)
        {
            //var sqlStoredProc = "sp_estimation_create";
            var sqlStoredProc = "sp_settle_item_create_or_modify";

            var response = await DapperAdapter.GetFromStoredProcAsync<int>
            (
                storedProcedureName: sqlStoredProc,
                parameters: new
                {
                    @SettleItemId =request.SettleItemId ,
                    @SettlementId = request.SettlementId,
                    @EstimationId = request.EstimationId,
                    @EstimateSettleItemId =request.EstimateSettleItemId,
                    @ActualQuantity = request.ActualQuantity,
                    @ActualUnitPrice = request.ActualUnitPrice,
                    @ActualTotalPrice = request.ActualTotalPrice,
                    @SettleItemRemarks = request.SettleItemRemarks,
                    @CreatedBy = request.CreatedBy
                },
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
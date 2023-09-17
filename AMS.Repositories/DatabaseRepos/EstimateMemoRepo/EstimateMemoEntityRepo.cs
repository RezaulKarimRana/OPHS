using AMS.Infrastructure.Adapters;
using AMS.Infrastructure.Configuration.Models;
using AMS.Models.CustomModels;
using AMS.Models.DomainModels;
using AMS.Repositories.DatabaseRepos.EstimateMemo.Contracts;
using AMS.Repositories.DatabaseRepos.EstimateMemo.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AMS.Repositories.DatabaseRepos.EmailContentsRepo.Models;
using AMS.Models.ServiceModels.Memo;
using Dapper;
using AMS.Models.ViewModel;
using AMS.Repositories.DatabaseRepos.MemoApproverRepo.Models;

namespace AMS.Repositories.DatabaseRepos.EstimateMemo
{
    public class EstimateMemoEntityRepo : BaseSQLRepo, IEstimateMemoEntityRepo
    {
        private readonly IDbConnection _connection;
        private readonly IDbTransaction _transaction;

        public EstimateMemoEntityRepo(IDbConnection connection, IDbTransaction transaction, ConnectionStringSettings connectionStringsSettings)
            : base(connectionStringsSettings)
        {
            _connection = connection;
            _transaction = transaction;
        }

        public async Task<int> CreateEstimationMemo(AddBudgetEstimationMemoDTO model, int userId)
        {
            try
            {
                var sqlStoredProc = "sp_create_estimation_memo";

                var approverList = new List<CreateMemoApproverRequest>();

                foreach(var item in model.EstimateApproverList)
                {
                    approverList.Add(new CreateMemoApproverRequest
                    {
                        UserId = item.User_Id,
                        PriorityId = item.Priority,
                        Remarks = item.Remarks,
                        RolePriorityId = item.RolePriority_Id,
                        Created_By = userId
                    });
                }

                var itemsDT = ConvertToDataTable(approverList);

                var request = new
                {
                    EstimateId = model.EstimateMemo.EstimateId,
                    EstimateReferenceId = model.EstimateMemo.EstimateReferId,
                    TotalDeviation = model.EstimateMemo.TotalDeviation,
                    Justification = model.EstimateMemo.JustificaitonText,
                    Created_By = userId,
                    itemlist = itemsDT.AsTableValuedParameter("ApproversDT")
                };

                var response = await DapperAdapter.GetFromStoredProcAsync<int>
                    (
                        storedProcedureName: sqlStoredProc,
                        parameters: request,
                        dbconnectionString: DefaultConnectionString,
                        sqltimeout: DefaultTimeOut,
                        dbconnection: _connection,
                        dbtransaction: _transaction
                    );
                return response.FirstOrDefault();
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public async Task<EstimationInfoForMemo> EstimationMemoInfoDetailsById(int id)
        {
            try
            {
                var sqlStoredProc = "sp_get_memo_details";

                var response = await DapperAdapter.GetFromStoredProcAsync<EstimationInfoForMemo>
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
            catch (Exception e)
            {

                throw;
            }
        }

        public async Task<EstimateMemoEntity> GetEstimateMemoEntityById(int id)
        {
            var sqlStoredProc = "sp_get_estimation_memo_by_id";

            var response = await DapperAdapter.GetFromStoredProcSingleAsync<EstimateMemoEntity>
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

        public async Task<IList<PendingMemo>> LoadAllPendingMemo()
        {
            var sqlStoredProc = "sp_load_all_pending_memo_for_initialization";

            var response = await DapperAdapter.GetFromStoredProcAsync<PendingMemo>
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

        public async Task<List<EstimationSettleItemDetailDTO>> LoadEstimationSettleItemDetailsByEstimationId(int estimationId)
        {
            var sqlStoredProc = "sp_load_estimation_settle_item_by_estimate_id";

            var response = await DapperAdapter.GetFromStoredProcAsync<EstimationSettleItemDetailDTO>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: new { @estimateId = estimationId },
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction
                );

            return response.ToList();
        }

        public async Task<List<EstimateSettleItemDetails>> LoadSettleItemDetailsBySettleItemAndEstimationId(int estimationId, int estimateSettleItem)
        {
            var sqlStoredProc = "sp_load_settle_item_detail_by_estimareSettlement";

            var response = await DapperAdapter.GetFromStoredProcAsync<EstimateSettleItemDetails>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: new { @estimation = estimationId , @settlementItem = estimateSettleItem},
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction
                );

            return response.ToList();
        }
        public async Task<EmailSenderInfo> getMemoRelatedEmailAddressByMemo(int memoId)
        {
            var sqlStoredProc = "sp_get_Comma_separate_email_info_by_memo_id";

            var response = await DapperAdapter.GetFromStoredProcAsync<EmailSenderInfo>
            (
                storedProcedureName: sqlStoredProc,
                parameters: new { @memoId = memoId },
                dbconnectionString: DefaultConnectionString,
                sqltimeout: DefaultTimeOut,
                dbconnection: _connection,
                dbtransaction: _transaction
            );
            return response.FirstOrDefault();
        }
        public async Task<MemoSummaryVM> LoadMemoSummary(int userId)
        {
            var sqlStoredProc = "sp_load_all_Memo_Summary";

            var response = await DapperAdapter.GetFromStoredProcAsync<MemoSummaryVM>
            (
                storedProcedureName: sqlStoredProc,
                parameters: new { @userId = userId },
                dbconnectionString: DefaultConnectionString,
                sqltimeout: DefaultTimeOut,
                dbconnection: _connection,
                dbtransaction: _transaction
            );
            return response.FirstOrDefault<MemoSummaryVM>();
        }
        public async Task<MemoSummaryVM> LoadApproverMemoSummary(int userId)
        {
            var sqlStoredProc = "sp_load_all_Memo_Summary_for_approver";

            var response = await DapperAdapter.GetFromStoredProcAsync<MemoSummaryVM>
            (
                storedProcedureName: sqlStoredProc,
                parameters: new { @userId = userId },
                dbconnectionString: DefaultConnectionString,
                sqltimeout: DefaultTimeOut,
                dbconnection: _connection,
                dbtransaction: _transaction
            );
            return response.FirstOrDefault<MemoSummaryVM>();
        }
        public async Task<List<MemoVM>> LoadMemoByStatus(int userId, int status, int currentPageIndex, int pAGE_SIZE)
        {
            var sqlStoredProc = "sp_load_all_Memo";

            var response = await DapperAdapter.GetFromStoredProcAsync<MemoVM>
            (
                storedProcedureName: sqlStoredProc,
                parameters: new { @user_Id = userId, @status = status, @start = currentPageIndex, @rowsperpage = pAGE_SIZE },
                dbconnectionString: DefaultConnectionString,
                sqltimeout: DefaultTimeOut,
                dbconnection: _connection,
                dbtransaction: _transaction
            );
            return response.ToList<MemoVM>();
        }
        public async Task<List<MemoVM>> LoadApproverMemoByStatus(int userId, int status, int currentPageIndex, int pAGE_SIZE)
        {
            var sqlStoredProc = "sp_load_all_Memo_for_approver";

            var response = await DapperAdapter.GetFromStoredProcAsync<MemoVM>
            (
                storedProcedureName: sqlStoredProc,
                parameters: new { @user_Id = userId, @status = status, @start = currentPageIndex, @rowsperpage = pAGE_SIZE },
                dbconnectionString: DefaultConnectionString,
                sqltimeout: DefaultTimeOut,
                dbconnection: _connection,
                dbtransaction: _transaction
            );
            return response.ToList<MemoVM>();
        }
    }
}

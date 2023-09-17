using AMS.Infrastructure.Adapters;
using AMS.Infrastructure.Configuration.Models;
using AMS.Models.CustomModels;
using AMS.Models.DomainModels;
using AMS.Repositories.DatabaseRepos.EstimateMemo.Models;
using AMS.Repositories.DatabaseRepos.EstimationRepo.Models;
using AMS.Repositories.DatabaseRepos.MemoApproverRepo.Contracts;
using AMS.Repositories.DatabaseRepos.MemoApproverRepo.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace AMS.Repositories.DatabaseRepos.MemoApproverRepo
{
    public class MemoApproverRepo : BaseSQLRepo, IMemoApproverRepo
    {

        private readonly IDbConnection _connection;
        private readonly IDbTransaction _transaction;

        public MemoApproverRepo(IDbConnection connection, IDbTransaction transaction, ConnectionStringSettings connectionStringsSettings)
            : base(connectionStringsSettings)
        {
            _connection = connection;
            _transaction = transaction;
        }

        public async Task<int> CheckIsValidToShowInMemoParking(int estimateMemoId, int priority)
        {
            try
            {
                var sqlStoredProc = "sp_load_all_pending_memo_approval_by_momo_and_priority";

                var response = await DapperAdapter.GetFromStoredProcSingleAsync<GetPendingCount>
                    (
                        storedProcedureName: sqlStoredProc,
                        parameters: new { @estimateMemoId = estimateMemoId, @priority = priority },
                        dbconnectionString: DefaultConnectionString,
                        sqltimeout: DefaultTimeOut,
                        dbconnection: _connection,
                        dbtransaction: _transaction
                    );
                return response.Count;
            }
            catch (Exception e)
            {

                throw;
            }
        }

        public async Task<int> CreateEstimateApproverForMemo(CreateMemoApproverRequest request)
        {
            try
            {
                var sqlStoredProc = "sp_create_estimate_memo_approver";

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
            catch (Exception e)
            {

                throw;
            }
        }

        public async Task<MemoApproverEntity> GetLatestPendingApproverByMemoIdAndUserId(int memoId, int userId)
        {
            try
            {
                var sqlStoredProc = "sp_get_top_pending_approver_by_memo_id_and_user_id";

                var response = await DapperAdapter.GetFromStoredProcSingleAsync<MemoApproverEntity>
                    (
                        storedProcedureName: sqlStoredProc,
                        parameters: new { @estimationMemoId = memoId, @userId = userId },
                        dbconnectionString: DefaultConnectionString,
                        sqltimeout: DefaultTimeOut,
                        dbconnection: _connection,
                        dbtransaction: _transaction
                    );

                return response;
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public async Task<List<MemoApproverEntity>> GetLatestPendingApproveresOfAMemo(int memoId)
        {
            try
            {
                var sqlStoredProc = "sp_memo_latest_pending_approvers_get";

                var response = await DapperAdapter.GetFromStoredProcAsync<MemoApproverEntity>
                    (
                        storedProcedureName: sqlStoredProc,
                        parameters: new { @memoId = memoId },
                        dbconnectionString: DefaultConnectionString,
                        sqltimeout: DefaultTimeOut,
                        dbconnection: _connection,
                        dbtransaction: _transaction
                    );

                return response.ToList();
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public async Task<IList<PendingMemo>> LoadAllPendingMemoApprovalForAUser(int user_Id)
        {
            try
            {
                var sqlStoredProc = "sp_load_all_pending_memo_approval_by_user_id";

                var response = await DapperAdapter.GetFromStoredProcAsync<PendingMemo>
                    (
                        storedProcedureName: sqlStoredProc,
                        parameters: new { @user_Id = user_Id },
                        dbconnectionString: DefaultConnectionString,
                        sqltimeout: DefaultTimeOut,
                        dbconnection: _connection,
                        dbtransaction: _transaction
                    );

                return response.ToList();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IList<EstimateMemoDetails>> LoadAllRunningMemoApprovalForAUser(int user_Id, 
            int pageNumber, int pageSize, string whereClaue)
        {
            try
            {
                var sql = @"Select distinct mm.Id Id,
	                            et.Name EstimateType, 
	                            mm.[Status] EstimateStatus,
	                            e.UniqueIdentifier EstimationIdentity,
	                            e.Subject,
	                            e.TotalPrice BudgetPrice, 
	                            eRefer.AllowableBudget AllowableBudget,
	                            eRefer.AlreadySettle FinalSettledAmount,
	                            mm.TotalDeviation TotalDeviation,
                                mm.Created_Date CreatedDateTime,
								mm.Created_By as [InitiatorId],
	                            (select count(*) from EstimateMemo where EstimateMemo.[Status] = 2 ) as 'TotalRow'
                            from [Estimation] e 
                            inner join EstimationReference eRefer on e.ID = eRefer.EstimationId
                            inner join EstimateMemo mm on mm.EstimateReferenceId = eRefer.Id
                            left join MemoApprover mmApp on mm.Id = mmApp.EstimateMemoId
                            left join MemoApproverFeedback mmFed on mmApp.ID = mmFed.MemoApproverId
                            left join [User] u on u.Id = mmApp.User_Id
                            left join [EstimateType] et on et.ID = e.EstimateType_Id

	                            where (mm.Created_By = "+ user_Id + @" or mmApp.User_Id = " + user_Id + @") 
	                            and mm.status= 2 " + whereClaue + @"
	                            order by mm.id
	                            OFFSET " + pageNumber + @" ROWS
	                            FETCH NEXT " + pageSize + " ROWS ONLY";
                var response = await DapperAdapter.ExecuteDynamicSql<EstimateMemoDetails>(
                    sql: sql,
                    dbconnectionString: DefaultConnectionString,
                    dbconnection: _connection,
                    sqltimeout: DefaultTimeOut,
                   dbtransaction: _transaction);

                return response.ToList();
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public async Task<List<ApproverDetailsDTO>> LoadApproverDetailsByMemo(int memoId)
        {
            var sql = @"Select 
                        mm.Id Id,
                        mmApp.ID ApproverId,
                        u.First_Name ApproverFirstName,
                        u.Last_Name ApproverLastName,
                        u.Username ApproverUserName,
                        mmApp.[Status] ApproverStatus,
                        mmApp.RolePriority_Id ApproverRoleId,
                        mmApp.[Priority] ApproverPriority,
                        mmFed.Created_Date ApproverFeedbackDate
                        from EstimateMemo mm 
                        left join MemoApprover mmApp on mm.Id = mmApp.EstimateMemoId
                        inner join [User] u on u.Id = mmApp.User_Id
                        left join MemoApproverFeedback mmFed on mmApp.ID = mmFed.MemoApproverId
                        where mm.Id = " + memoId + @" order by mmApp.[Priority] desc";

            var response = await DapperAdapter.ExecuteDynamicSql<ApproverDetailsDTO>(
                    sql: sql,
                    dbconnectionString: DefaultConnectionString,
                    dbconnection: _connection,
                    sqltimeout: DefaultTimeOut,
                   dbtransaction: _transaction);

            return response.ToList();
        }

        public async Task<List<MemoApproverEntity>> LoadLatestPendingMemoApproverByMemoId(int memoId)
        {
            try
            {
                var sqlStoredProc = "sp_get_memo_latest_pending_approvers";

                var response = await DapperAdapter.GetFromStoredProcAsync<MemoApproverEntity>
                    (
                        storedProcedureName: sqlStoredProc,
                        parameters: new { @memoId = memoId },
                        dbconnectionString: DefaultConnectionString,
                        sqltimeout: DefaultTimeOut,
                        dbconnection: _connection,
                        dbtransaction: _transaction
                    );
                return response.ToList();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<List<MemoApproverDetailsDTO>> LoadMemoApproverDetailsByMemoId(int memoId)
        {
            try
            {
                var sqlStoredProc = "sp_load_memo_approver_by_memo_id";

                var response = await DapperAdapter.GetFromStoredProcAsync<MemoApproverDetailsDTO>
                    (
                        storedProcedureName: sqlStoredProc,
                        parameters: new { @memoId = memoId },
                        dbconnectionString: DefaultConnectionString,
                        sqltimeout: DefaultTimeOut,
                        dbconnection: _connection,
                        dbtransaction: _transaction
                    );
                return response.ToList();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<List<LoadMemoApproverFeedBackDetails>> LoadMemoApproverFeedBackDetails(int memoId)
        {
            try
            {
                var sqlStoredProc = "sp_load_memo_approver_feedBack_details";

                var response = await DapperAdapter.GetFromStoredProcAsync<LoadMemoApproverFeedBackDetails>
                    (
                        storedProcedureName: sqlStoredProc,
                        parameters: new { @memoId = memoId },
                        dbconnectionString: DefaultConnectionString,
                        sqltimeout: DefaultTimeOut,
                        dbconnection: _connection,
                        dbtransaction: _transaction
                    );

                return response.ToList();
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public async Task UpdateMemoApproverOnApproval(UpdateMemoApprover requestDto)
        {
            var sqlStoredProc = "sp_memo_status_update";

            var response = await DapperAdapter.GetFromStoredProcAsync<int>
            (
                storedProcedureName: sqlStoredProc,
                parameters: requestDto,
                dbconnectionString: DefaultConnectionString,
                sqltimeout: DefaultTimeOut,
                dbconnection: _connection,
                dbtransaction: _transaction
            );

            if (response == null)
            {
                throw new Exception("No items have been updated");
            }
        }
    }
}

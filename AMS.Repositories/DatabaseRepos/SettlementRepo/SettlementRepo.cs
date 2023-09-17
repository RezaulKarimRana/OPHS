using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AMS.Infrastructure.Adapters;
using AMS.Infrastructure.Configuration.Models;
using AMS.Models.CustomModels;
using AMS.Models.DomainModels;
using AMS.Models.ServiceModels.Settlement;
using AMS.Repositories.DatabaseRepos.EmailContentsRepo.Models;
using AMS.Repositories.DatabaseRepos.SettlementRepo.Models;

namespace AMS.Repositories.DatabaseRepos.SettlementRepo
{
    public class SettlementRepo : BaseSQLRepo, ISettlementRepo
    {
        private readonly IDbConnection _connection;
        private readonly IDbTransaction _transaction;

        public SettlementRepo(IDbConnection connection, IDbTransaction transaction,
            ConnectionStringSettings connectionStringsSettings)
            : base(connectionStringsSettings)
        {
            _connection = connection;
            _transaction = transaction;
        }

        public async Task<int> CreateOrModifySettlement(CreateSettlementRequest request)
        {
            
            try
            {
                var sqlStoredProc = "sp_settlement_create_or_modify";

                var  response = await DapperAdapter.GetFromStoredProcAsync<int>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: new
                    {
                        @Id = request.Id,
                        @Estimate_Id = request.EstimationId,
                        @Status = request.Status,
                        @SettlementNote = request.SettlementNote,
                        @Created_By = request.CreatedBy,
                        @totalSettlementAmount = request.TotalAmount,
                        @isItFinalSettlement = request.IsItFinalSetttlement
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
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

            return 0;


        }
        public async Task<CreateSettlementRequest> getDraftSettlmentId(int estimateId, int userId)
        {
            
            var sqlStoredProc = "sp_get_settlement_By_Status";

            var response = await DapperAdapter.GetFromStoredProcAsync<CreateSettlementRequest>
            (
                storedProcedureName: sqlStoredProc,
                parameters: new {@UserId = userId, @estimateId = estimateId, @Status = 5},
                dbconnectionString: DefaultConnectionString,
                sqltimeout: DefaultTimeOut,
                dbconnection: _connection,
                dbtransaction: _transaction
            );


            return response.FirstOrDefault();
        }
        public async Task<CreateSettlementRequest> getSettlementBySettlementId(int settlementId, int userId)
        {
         
            var sqlStoredProc = "sp_get_settlement_By_SettlementId_and_UserId";

            var response = await DapperAdapter.GetFromStoredProcAsync<CreateSettlementRequest>
            (
                storedProcedureName: sqlStoredProc,
                parameters: new { @UserId = userId, @SettlementId = settlementId },
                dbconnectionString: DefaultConnectionString,
                sqltimeout: DefaultTimeOut,
                dbconnection: _connection,
                dbtransaction: _transaction
            );


            return response.FirstOrDefault();
        }
        public async Task<int> CreateSettlementApprover(CreateSettlementApproverRequest request)
        {
            var sqlStoredProc = "sp_settlement_approver_create";

            var response = await DapperAdapter.GetFromStoredProcAsync<int>
            (
                storedProcedureName: sqlStoredProc,
                parameters: new {@Settlement_Id = request.Settlement_Id,
                @User_Id = request.User_Id,
                @Priority = request.Priority,
                @Status = request.Status,
                @Remarks = request.Remarks,
                @RolePriority_Id = request.RolePriority_Id,
                @Created_By = request.Created_By,
                @PlanDate = request.PlanDate
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
        public async Task<int> UpdateSettlementFeedback(SettlementFeedback request)
        {
            var sqlStoredProc = "sp_change_settlement_and_related_data";

            var response = await DapperAdapter.GetFromStoredProcAsync<int>
            (
                storedProcedureName: sqlStoredProc,
                parameters: new
                {
                    @UserId = request.UserId,
                    @SettlementId = request.SettlementId,
                    @RolePiority = request.CurrentUserRolePiority,
                    @Remarks = request.Remarks,
                    @Feedback = request.Feedback
                  
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
        public async Task<List<SettlementVM>> LoadAllSettlementForFollower(int userId)
        {
            var sqlStoredProc = "sp_load_all_settlement_for_follower_by_follower_id";

            var response = await DapperAdapter.GetFromStoredProcAsync<SettlementVM>
            (
                storedProcedureName: sqlStoredProc,
                parameters: new { @user_Id = userId },
                dbconnectionString: DefaultConnectionString,
                sqltimeout: DefaultTimeOut,
                dbconnection: _connection,
                dbtransaction: _transaction
            );
            return response.ToList<SettlementVM>();
        }
        public async Task<List<SettlementApproverDetails>> LoadSettlementApproverDetailsBySettlementId(int settlementId)
        {
            var sqlStoredProc = "sp_load_settlement_approver_by_settlement_id";

            var response = await DapperAdapter.GetFromStoredProcAsync<SettlementApproverDetails>
            (
                storedProcedureName: sqlStoredProc,
                parameters: new { @settlementId = settlementId },
                dbconnectionString: DefaultConnectionString,
                sqltimeout: DefaultTimeOut,
                dbconnection: _connection,
                dbtransaction: _transaction
            );
            return response.ToList();
        }
        public async Task<List<SettlementApproverFeedbackDetails>> LoadSettlementApproverFeedBackDetailsBySettlementId(int settlementId)
        {
            var sqlStoredProc = "sp_load_settlement_approver_feedBack_details";

            var response = await DapperAdapter.GetFromStoredProcAsync<SettlementApproverFeedbackDetails>
            (
                storedProcedureName: sqlStoredProc,
                parameters: new { @settlementId = settlementId },
                dbconnectionString: DefaultConnectionString,
                sqltimeout: DefaultTimeOut,
                dbconnection: _connection,
                dbtransaction: _transaction
            );
            return response.ToList();
        }
        public async Task<List<SettlementVM>> LoadSettlementByStatus(int userId, int status, int currentPageIndex, int pAGE_SIZE)
        {
            var sqlStoredProc = "sp_load_all_Settlement";

            var response = await DapperAdapter.GetFromStoredProcAsync<SettlementVM>
            (
                storedProcedureName: sqlStoredProc,
                parameters: new { @user_Id = userId, @status = status, @start = currentPageIndex, @rowsperpage = pAGE_SIZE },
                dbconnectionString: DefaultConnectionString,
                sqltimeout: DefaultTimeOut,
                dbconnection: _connection,
                dbtransaction: _transaction
            );
            return response.ToList<SettlementVM>();
        }
        public async Task<SettlementSummaryVM> LoadSettlementSummary(int userId)
        {
            var sqlStoredProc = "sp_load_all_Settlement_Summary";

            var response = await DapperAdapter.GetFromStoredProcAsync<SettlementSummaryVM>
            (
                storedProcedureName: sqlStoredProc,
                parameters: new { @userId = userId },
                dbconnectionString: DefaultConnectionString,
                sqltimeout: DefaultTimeOut,
                dbconnection: _connection,
                dbtransaction: _transaction
            );
            return response.FirstOrDefault<SettlementSummaryVM>();
        }
        public async Task<List<SettlementApproverDetails>> GetAllApproverBySettlementIdForRollbackDraft(int settlementId)
        {
            var sqlStoredProc = "sp_load_estimate_approver_by_settlement_id_rollback_draft";

            var response = await DapperAdapter.GetFromStoredProcAsync<SettlementApproverDetails>
            (
                storedProcedureName: sqlStoredProc,
                parameters: new { @settlementId = settlementId },
                dbconnectionString: DefaultConnectionString,
                sqltimeout: DefaultTimeOut,
                dbconnection: _connection,
                dbtransaction: _transaction
            );
            return response.ToList();
        }
        public async Task<List<SettlementVM>> getInCompletedSettlementForCheckFinalSettlement(int userId, int departmentId,int estimationId)
        {
            var sqlStoredProc = "sp_get_InCompleted_Settlement_For_Check_FinalSettlement";

            var response = await DapperAdapter.GetFromStoredProcAsync<SettlementVM>
            (
                storedProcedureName: sqlStoredProc,
                parameters: new { @UserId = userId, @DepartmentID = departmentId, @estimationId = estimationId },
                dbconnectionString: DefaultConnectionString,
                sqltimeout: DefaultTimeOut,
                dbconnection: _connection,
                dbtransaction: _transaction
            );

            return response.ToList();
        }
        public async Task<int> CreateAttachmentForSettlement(CreateAttachmentForSettlementRequest request)
        {
            var sqlStoredProc = "sp_create_estimation_settlement_attachment";

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
        public async Task<List<EstimateSettlementAttachmentsEntity>> LoadSettlementAttachmentsBySettlementId(int estimateSettlementId)
        {
            try
            {
                var sqlStoredProc = "sp_load_estimate_settlement_attachments_by_settlement";

                var response = await DapperAdapter.GetFromStoredProcAsync<EstimateSettlementAttachmentsEntity>
                    (
                        storedProcedureName: sqlStoredProc,
                        parameters: new { @estimateSettlementId = estimateSettlementId },
                        dbconnectionString: DefaultConnectionString,
                        sqltimeout: DefaultTimeOut,
                        dbconnection: _connection,
                        dbtransaction: _transaction
                    );

                return response.ToList();
            }
            catch (Exception e)
            {

                throw;
            }
        }
        public async Task<CreateSettlementRequest> getSettlementById(int settlementId)
        {

            var sqlStoredProc = "sp_get_settlement_By_SettlementId";

            var response = await DapperAdapter.GetFromStoredProcAsync<CreateSettlementRequest>
            (
                storedProcedureName: sqlStoredProc,
                parameters: new { @SettlementId = settlementId },
                dbconnectionString: DefaultConnectionString,
                sqltimeout: DefaultTimeOut,
                dbconnection: _connection,
                dbtransaction: _transaction
            );


            return response.FirstOrDefault();
        }
        public async Task<EmailSenderInfo> getSettlementRelatedEmailAddressBySettlement(int settlementId)
        {
            var sqlStoredProc = "sp_get_Comma_separate_email_info_by_settlement";

            var response = await DapperAdapter.GetFromStoredProcAsync<EmailSenderInfo>
            (
                storedProcedureName: sqlStoredProc,
                parameters: new { @SettlementId = settlementId },
                dbconnectionString: DefaultConnectionString,
                sqltimeout: DefaultTimeOut,
                dbconnection: _connection,
                dbtransaction: _transaction
            );
            return response.FirstOrDefault();
        }
        public async Task<List<ReadyForSettlementVM>> ReadyToSettlementList(int userId, int currentPageIndex, int pAGE_SIZE, int departmentId)
        {
            var sqlStoredProc = "sp_settlement_ready_list";

            var response = await DapperAdapter.GetFromStoredProcAsync<ReadyForSettlementVM>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: new { @UserId = userId, @start = currentPageIndex, @rowsperpage = pAGE_SIZE, @DepartmentID = departmentId, @currentstatus = BaseEntity.EntityStatus.Completed },
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction
                );
            return response.ToList();
        }
        public async Task<bool> DeleteAttachmentsById(int id)
        {
            var sqlStoredProc = "sp_Settlement_Attachments_DeleteById";

            var response = await DapperAdapter.GetFromStoredProcAsync<int>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: new { id },
                    dbconnectionString: _connection.ConnectionString,
                    sqltimeout: _connection.ConnectionTimeout,
                    dbconnection: _connection,
                    dbtransaction: _transaction
                );
            return response.Count() > 0 ? true : false;
        }
    }
}
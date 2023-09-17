using AMS.Infrastructure.Adapters;
using AMS.Infrastructure.Configuration.Models;
using AMS.Models.CustomModels;
using AMS.Models.DomainModels;
using AMS.Models.ServiceModels.BudgetEstimate;
using AMS.Repositories.DatabaseRepos.EstimationRepo.Contracts;
using AMS.Repositories.DatabaseRepos.EstimationRepo.Models;
using AMS.Repositories.DatabaseRepos.ProcurementApproval.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace AMS.Repositories.DatabaseRepos.EstimationRepo
{
    public class EstimationRepo : BaseSQLRepo, IEstimationRepo
    {
        private readonly IDbConnection _connection;
        private readonly IDbTransaction _transaction;

        public EstimationRepo(IDbConnection connection, IDbTransaction transaction, ConnectionStringSettings connectionStringsSettings)
            : base(connectionStringsSettings)
        {
            _connection = connection;
            _transaction = transaction;
        }

        public async Task<int> CheckIsValidToShowInParking(int estimationId, int priority)
        {
            var sqlStoredProc = "sp_load_all_pending_budget_approval_by_estimation_and_priority";

            var response = await DapperAdapter.GetFromStoredProcSingleAsync<GetPendingCount>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: new { @estimationId = estimationId, @priority = priority },
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction
                );
            return response.Count;
        }

        public async Task CompleteBudget(int id, int userId, int statusCode)
        {
            var sql = @" update [Estimation] set status ='" + statusCode + "',[Updated_Date]= GETDATE()," +
                      "[Updated_By]=" + userId + " where id = " + id;
            await DapperAdapter.ExecuteDynamicSql<int>(
                sql: sql,
                dbconnectionString: DefaultConnectionString,
                dbconnection: _connection,
                sqltimeout: DefaultTimeOut,
                dbtransaction: _transaction);
        }

        public async Task<int> CreateEstimation(CreateEstimationRequest request)
        {
            var sqlStoredProc = "sp_estimation_create";

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

        public async Task DeleteEstimation(DeleteEstimationRequest request)
        {
            var sqlStoredProc = "sp_estimation_delete";

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

        public async Task DisabledEstimation(DisableEstimation request)
        {
            var sqlStoredProc = "sp_disable_one_estimation";

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

        public async Task<List<DraftedBudgetEstimationByUser>> LoadDraftEstimation(int userId, int start, int pAGE_SIZE)
        {
            var sqlStoredProc = "sp_load_all_drafted_budget_approval_by_user";

            var response = await DapperAdapter.GetFromStoredProcAsync<DraftedBudgetEstimationByUser>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: new { @user_Id = userId, @start = start, @rowsperpage = pAGE_SIZE, @currentstatus = BaseEntity.EntityStatus.Draft },
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction
                );

            return response.ToList();
        }

        public async Task<List<DraftedBudgetEstimationByUser>> LoadCREstimation(int userId, int start, int pAGE_SIZE)
        {
            var sqlStoredProc = "sp_load_all_drafted_budget_approval_by_user";

            var response = await DapperAdapter.GetFromStoredProcAsync<DraftedBudgetEstimationByUser>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: new { @user_Id = userId, @start = start, @rowsperpage = pAGE_SIZE, @currentstatus = BaseEntity.EntityStatus.CR },
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction
                );

            return response.ToList();
        }

        public async Task<List<EstimationEntity>> GetAllEstimation()
        {
            var sqlStoredProc = "sp_all_estimation_get";

            var response = await DapperAdapter.GetFromStoredProcAsync<EstimationEntity>
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

        public async Task<List<GetAllEstimationSubjectNames>> GetAllEstimationSubjectNameList()
        {
            var sqlStoredProc = "sp_all_estimation_names_get";

            var response = await DapperAdapter.GetFromStoredProcAsync<GetAllEstimationSubjectNames>
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

        public async Task<EstimationEntity?> GetById(int id)
        {
            var sqlStoredProc = "sp_estimate_get_by_id";

            var response = await DapperAdapter.GetFromStoredProcSingleAsync<EstimationEntity>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: new { id },
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction
                );

            return response;
        }

        public async Task<EstimationEntity> GetSingleEstimation(int id)
        {
            var sqlStoredProc = "sp_draft_estimation_get_by_id";

            var response = await DapperAdapter.GetFromStoredProcAsync<EstimationEntity>
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

        public async Task<List<EstimateEditVM>> LoadAllPendingEstimate(int userId, int currentPageIndex, int pAGE_SIZE, int UserId)
        {
            var sqlStoredProc = "sp_load_all_pending_budget_for_statusboard";

            var response = await DapperAdapter.GetFromStoredProcAsync<EstimateEditVM>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: new { @user_Id = userId, @start = currentPageIndex, @rowsperpage = pAGE_SIZE, @UserId, @currentstatus = BaseEntity.EntityStatus.Pending },
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction
                );
            return response.ToList<EstimateEditVM>();
        }

        public async Task<List<EstimateEditVM>> LoadAllCompleteEstimate(int userId,
            int currentPageIndex, int pAGE_SIZE, int UserId, string whereClause, bool IsNotForCount = true)
        {
            try
            {

                var sql = @"Select distinct e.id Id, 
                        e.Updated_Date,
	                    et.Name EstimateType, 
	                    e.Status ,
	                    p.Name ProjectName, 
	                    e.UniqueIdentifier EstimationIdentity,
	                    e.Subject,
	                    e.Objective,
	                    e.Details,
	                    e.PlanStartDate,
	                    e.PlanEndDate,
	                    e.Remarks,
	                    e.TotalPrice,
                        e.TotalPriceCurrencyType as CurrencyType,
	                    --eapp.Priority, 
	                    (select count(*) from [Estimation] where [Estimation].status= 100 ) as 'TotalRow'
			                    from [Estimation] e 
	                    left join [EstimateDetails] ed on e.id = ed.Estimation_Id
	                    left join [EstimateApprover] eapp on eapp.[Estimate_Id]= e.ID
	                    left join [EstimateApproverFeedback] eappf on eappf.EstimateApprover_Id = eapp.ID
	                    left join [Item] i on i.ID = ed.Item_Id
	                    left join [User] u on u.Id = eapp.User_Id
	                    left join [EstimateType] et on et.ID = e.EstimateType_Id
	                    left join [Project] p on p.ID = e.Project_Id
	                    where 

                         e.status= 100 " + whereClause + @"
	                    order by e.Updated_Date desc";

                if (IsNotForCount)
                {
                    sql += @" OFFSET " + currentPageIndex + @" ROWS
	                    FETCH NEXT " + pAGE_SIZE + @" ROWS ONLY";
                }

                var response = await DapperAdapter.ExecuteDynamicSql<EstimateEditVM>(
                    sql: sql,
                    dbconnectionString: DefaultConnectionString,
                    dbconnection: _connection,
                    sqltimeout: DefaultTimeOut,
                    dbtransaction: _transaction);

                return response.ToList();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<EstimateVM>> LoadAllPendingEstimateByUser(int userId)
        {
            var sqlStoredProc = "sp_load_all_pending_budget_approval_by_user_id";

            var response = await DapperAdapter.GetFromStoredProcAsync<EstimateVM>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: new { @user_Id = userId },
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction
                );
            return response.ToList<EstimateVM>();
        }

        public async Task<EstimationWithEstimationType> SingleEstimationWithType(int estiId)
        {
            var sqlStoredProc = "sp_estimation_with_type_get";

            var response = await DapperAdapter.GetFromStoredProcAsync<EstimationWithEstimationType>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: new { EstimateId = estiId },
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction
                );

            return response.FirstOrDefault();
        }

        public async Task UpdateEstimation(UpdateEstimationRequest request)
        {
            var sqlStoredProc = "sp_estimation_update";

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

        public async Task UpdateTotalPrice(UpdateEstimateTotalPriceRequest request)
        {
            var sqlStoredProc = "sp_estimate_total_price_update";

            var response = await DapperAdapter.GetFromStoredProcAsync<int>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: request,
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

        public async Task IncreaseIncrementalValue(int updatedUserId)
        {
            var sqlStoredProc = "sp_autoIncrementedValueTable_update";

            var response = await DapperAdapter.GetFromStoredProcAsync<int>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: new { @updatedBy = updatedUserId},
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

        public async Task<AutoIncrementedValueTableEntity> GetIncrementedValue()
        {
            var sqlStoredProc = "sp_autoIncrementedValueTable_get";

            var response = await DapperAdapter.GetFromStoredProcAsync<AutoIncrementedValueTableEntity>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: new { },
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction
                );

            return response.FirstOrDefault();
        }

        public async Task<List<EstimationSearch>> GetEstimateSearch(string keyWord, int userID)
        {
            var sqlStoredProc = "sp_estimation_search";

            var response = await DapperAdapter.GetFromStoredProcAsync<EstimationSearch>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: new { @keyWord = keyWord, @userID = userID },
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction
                );
            return response.ToList<EstimationSearch>();
        }

        public async Task<List<EstimateEditVM>> LoadRejectedEstimate(int userId, int currentPageIndex, int pAGE_SIZE, int UserId)
        {
            var sqlStoredProc = "sp_load_all_rejected_budget_for_statusboard";

            var response = await DapperAdapter.GetFromStoredProcAsync<EstimateEditVM>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: new { @user_Id = userId, @start = currentPageIndex, @rowsperpage = pAGE_SIZE, @UserId, @currentstatus = BaseEntity.EntityStatus.Reject },
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction
                );
            return response.ToList<EstimateEditVM>();
        }

        public async Task<List<EstimateEditVM>> LoadAllCompleteEstimateExceptFinance(int userId, 
            int currentPageIndex, int pAGE_SIZE, int UserId, string whereClause, bool IsNotForCount = true)
        {
            try
            {
                //var sqlStoredProc = "sp_load_all_completed_budget_except_for_finance_for_statusboard";

                //var response = await DapperAdapter.GetFromStoredProcAsync<EstimateEditVM>
                //    (
                //        storedProcedureName: sqlStoredProc,
                //        parameters: new { @user_Id = userId, @start = currentPageIndex, @rowsperpage = pAGE_SIZE, @UserId, @currentstatus = BaseEntity.EntityStatus.Completed },
                //        dbconnectionString: DefaultConnectionString,
                //        sqltimeout: DefaultTimeOut,
                //        dbconnection: _connection,
                //        dbtransaction: _transaction
                //    );
                var sql = @"select distinct
    et.IsApplicableForCompletedList,
    e.id                        Id,
                               et.Name                     EstimateType,
                               et.id                       EstimateTypeId,
                               e.Status,

                               e.UniqueIdentifier          EstimationIdentity,
                               e.Subject,
                               e.Objective,
                               e.Details,
                               e.PlanStartDate,
                               e.PlanEndDate,
                               e.Remarks,
                               e.TotalPrice,
                                e.Updated_Date,
                               e.TotalPriceCurrencyType as CurrencyType
,e.Status


     from Estimation e
           left join EstimateApprover ea on ea.Estimate_Id = e.ID
     join EstimateType et on et.id = e.EstimateType_Id
where
  
    (e.Status=100 and  (ea.User_Id= " + UserId + @" or e.Created_By= " + UserId + @" ))   and
        (
            (  et.IsApplicableForCompletedList = 0)
            or
            (
                 et.IsApplicableForCompletedList = 1
                    and " + UserId + @" in ( 
                                    select UserId from ListControllConfiguration where IsDisabled =0
                                )
            )
        ) " + whereClause + @"order by e.Updated_Date desc ";

                //var sql = @"Select distinct e.id Id, 
                //            e.Updated_Date,
                //         et.Name EstimateType, 
                //         e.Status ,
                //         p.Name ProjectName, 
                //         e.UniqueIdentifier EstimationIdentity,
                //         e.Subject,
                //         e.Objective,
                //         e.Details,
                //         e.PlanStartDate,
                //         e.PlanEndDate,
                //         e.Remarks,
                //         e.TotalPrice, 
                //            e.TotalPriceCurrencyType as CurrencyType
                //         (select count(*) from [Estimation]  left join [EstimateApprover] on Estimation.ID = EstimateApprover.Estimate_Id where [Estimation].status= 100 and (([Estimation].Created_By = " + userId + @" or EstimateApprover.User_Id = " + UserId + @")) ) as 'TotalRow'
                //           from [Estimation] e 
                //         left join [EstimateDetails] ed on e.id = ed.Estimation_Id
                //         left join [EstimateApprover] eapp on eapp.[Estimate_Id]= e.ID
                //         left join [EstimateApproverFeedback] eappf on eappf.EstimateApprover_Id = eapp.ID
                //         left join [Item] i on i.ID = ed.Item_Id
                //         left join [User] u on u.Id = eapp.User_Id
                //         left join [EstimateType] et on et.ID = e.EstimateType_Id
                //         left join [Project] p on p.ID = e.Project_Id
                //         where (e.Created_By = " + UserId + " or eapp.User_Id = " + UserId + @")

                //            and e.status= 100 " + whereClause + @"
                //         order by e.Updated_Date desc";

                if (IsNotForCount)
                {
                    sql += @" OFFSET " + currentPageIndex + @" ROWS
                     FETCH NEXT " + pAGE_SIZE + @" ROWS ONLY";
                }
                var response = await DapperAdapter.ExecuteDynamicSql<EstimateEditVM>(
                    sql: sql,
                    dbconnectionString: DefaultConnectionString,
                    dbconnection: _connection,
                    sqltimeout: DefaultTimeOut,
                    dbtransaction: _transaction);

                return response.ToList();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<DraftedBudgetEstimationByUser>> LoadOngoinEstimationByUser(int userId)
        {
            var sqlStoredProc = "sp_load_all_ongoing_budget_approval_involved_by_user";

            var response = await DapperAdapter.GetFromStoredProcAsync<DraftedBudgetEstimationByUser>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: new { @user_Id = userId },
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction
                );

            return response.ToList();
        }

        public async Task<List<EstimateEditVM>> LoadAllApprovedEstimateByUserDepartment(int userId, int currentPageIndex, int pAGE_SIZE, int UserId, int departmentId)
        {
            var sqlStoredProc = "sp_load_all_approved_estimate_by_user_department";

            var response = await DapperAdapter.GetFromStoredProcAsync<EstimateEditVM>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: new { @user_Id = UserId, @start = currentPageIndex, @rowsperpage = pAGE_SIZE, @UserId = UserId, @DepartmentID=departmentId, @currentstatus = BaseEntity.EntityStatus.Completed },
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction
                );
            return response.ToList<EstimateEditVM>();
        }

        public async Task<List<EstimateEditVM>> LoadAllApprovedEstimateforSettlement(int userId, int currentPageIndex, int pAGE_SIZE, int UserId, int departmentId)
        {
            var sqlStoredProc = "sp_load_all_approved_estimate_by_user_department";

            var response = await DapperAdapter.GetFromStoredProcAsync<EstimateEditVM>
            (
                storedProcedureName: sqlStoredProc,
                parameters: new { @user_Id = userId, @start = currentPageIndex, @rowsperpage = pAGE_SIZE, @UserId = UserId, @DepartmentID = departmentId, @currentstatus = BaseEntity.EntityStatus.Completed },
                dbconnectionString: DefaultConnectionString,
                sqltimeout: DefaultTimeOut,
                dbconnection: _connection,
                dbtransaction: _transaction
            );
            return response.ToList<EstimateEditVM>();
        }

        public async Task<EstimationInfoForMemo> EstimationInfo(int estiId)
        {
            var sqlStoredProc = "sp_estimation_info_for_memo";

            var response = await DapperAdapter.GetFromStoredProcAsync<EstimationInfoForMemo>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: new { EstimateId = estiId },
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction
                );

            return response.FirstOrDefault();
        }


        public async Task<List<EstimateVM>> LoadAllEstimateByUserExceptPending(int userId)
        {
            var sqlStoredProc = "sp_load_all_budget_approval_by_user_id_except_pending";

            var response = await DapperAdapter.GetFromStoredProcAsync<EstimateVM>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: new { @user_Id = userId },
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction
                );
            return response.ToList<EstimateVM>();
        }

        public async Task<List<EstimateVM>> LoadAllRejectedEstimateBySpecificUser(int userId)
        {
            var sqlStoredProc = "sp_load_all_rejected_budget_for_specific_user";

            var response = await DapperAdapter.GetFromStoredProcAsync<EstimateVM>
            (
                storedProcedureName: sqlStoredProc,
                parameters: new { @user_Id = userId },
                dbconnectionString: DefaultConnectionString,
                sqltimeout: DefaultTimeOut,
                dbconnection: _connection,
                dbtransaction: _transaction
            );
            return response.ToList<EstimateVM>();
        }

        public async Task<List<EstimateVM>> LoadAllRunningEstimateBySpecificUser(int userId)
        {
            var sqlStoredProc = "sp_load_all_running_budget_approval_by_user_id_for_secific_user";

            var response = await DapperAdapter.GetFromStoredProcAsync<EstimateVM>
            (
                storedProcedureName: sqlStoredProc,
                parameters: new { @user_Id = userId },
                dbconnectionString: DefaultConnectionString,
                sqltimeout: DefaultTimeOut,
                dbconnection: _connection,
                dbtransaction: _transaction
            );
            return response.ToList<EstimateVM>();
        }

        public async Task<List<EstitmationApprovalInfo>> load_all_approval_data_forUserByUserID(int userId)
        {
            var sqlStoredProc = "sp_load_all_approval_data_forUserByUserID";

            var response = await DapperAdapter.GetFromStoredProcAsync<EstitmationApprovalInfo>
            (
                storedProcedureName: sqlStoredProc,
                parameters: new { @user_Id = userId },
                dbconnectionString: DefaultConnectionString,
                sqltimeout: DefaultTimeOut,
                dbconnection: _connection,
                dbtransaction: _transaction
            );
            return response.ToList<EstitmationApprovalInfo>();
        }

        public async Task<List<DashboardTotalAmountVM>> GetAllBudgetAmountSumByUserId(int userId)
        {
            var sqlStoredProc = "Sp_get_total_amount_sum_by_user";

            var response = await DapperAdapter.GetFromStoredProcAsync<DashboardTotalAmountVM>
            (
                storedProcedureName: sqlStoredProc,
                parameters: new { @user_Id = userId },
                dbconnectionString: DefaultConnectionString,
                sqltimeout: DefaultTimeOut,
                dbconnection: _connection,
                dbtransaction: _transaction
            );
            return response.ToList<DashboardTotalAmountVM>();
        }
    }
}

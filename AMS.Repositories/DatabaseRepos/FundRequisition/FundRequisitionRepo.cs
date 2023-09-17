using AMS.Common.Constants;
using AMS.Infrastructure.Adapters;
using AMS.Infrastructure.Configuration.Models;
using AMS.Models.ServiceModels.FundRequisition;
using System;
using System.Collections.Generic;

using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMS.Models.CustomModels;

namespace AMS.Repositories.DatabaseRepos.FundRequisition
{
    public class FundRequisitionRepo : BaseSQLRepo, IFundRequisitionRepo
    {
        private readonly IDbConnection _connection;
        private readonly IDbTransaction _transaction;

        public FundRequisitionRepo(IDbConnection connection, IDbTransaction transaction, ConnectionStringSettings connectionStringsSettings)
            : base(connectionStringsSettings)
        {
            _connection = connection;
            _transaction = transaction;
        }


        public async Task<int> CreateFundRequisition(Models.DomainModels.FundRequisition request)
        {
            var sqlStoredProc = "sp_create_fundRequisition";

            var response = await DapperAdapter.GetFromStoredProcAsync<int>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters:  new { 
                        @Type = request.Type,
                        @RequisitionStatus = request.RequisitionStatus, 
                        @Amount = request.Amount,
                        @ProposedDisburseDate  = request.ProposedDisburseDate,
                        @Remarks  = request.Remarks,
                        @EstimationId = request.EstimationId,
                        @Created_By = request.Created_By,
                        @Updated_By = request.Updated_By,
                        @Created_Date = request.Created_Date,
                        @Updated_Date = request.Updated_Date
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

        public async Task<List<FundRequisitionVM>> SubmitedFundRequistionList(int userId, int currentPageIndex, int pAGE_SIZE, int UserId, int departmentId)
        {
            var sqlStoredProc = "sp_load_all_Submitted_fund_requisiotn";

            var response = await DapperAdapter.GetFromStoredProcAsync<FundRequisitionVM>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: new { @user_Id = userId, @start = currentPageIndex, @rowsperpage = pAGE_SIZE, @UserId = UserId, @DepartmentID = departmentId, @currentstatus = FundRequisitionStatus.Pending },
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction
                );
            return response.ToList<FundRequisitionVM>();
        }

        public async Task<List<FundRequisitionVM>> FundRequistionListByStatus(int userId, int currentPageIndex, int pAGE_SIZE, int UserId, int departmentId, int status)
        {
            var sqlStoredProc = "sp_load_all_fund_requisiotn_by_status";

            var response = await DapperAdapter.GetFromStoredProcAsync<FundRequisitionVM>
            (
                storedProcedureName: sqlStoredProc,
                parameters: new { @user_Id = userId, @start = currentPageIndex, @rowsperpage = pAGE_SIZE, @UserId = UserId, @DepartmentID = departmentId, @currentstatus = status },
                dbconnectionString: DefaultConnectionString,
                sqltimeout: DefaultTimeOut,
                dbconnection: _connection,
                dbtransaction: _transaction
            );
            return response.ToList<FundRequisitionVM>();
        }
        //

        public async Task<List<FundRequisitionVM>> FundRequistionListForFinance(int userId, int currentPageIndex, int pAGE_SIZE, int UserId, int departmentId)
        {
          
            var sqlStoredProc = "sp_fund_requisition_list_for_finance_by_status";

            var response = await DapperAdapter.GetFromStoredProcAsync<FundRequisitionVM>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: new { @user_Id = userId, @start = currentPageIndex, @rowsperpage = pAGE_SIZE, @UserId = UserId, @DepartmentID = departmentId, @currentstatus = FundRequisitionStatus.Pending },
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction
                );
            return response.ToList<FundRequisitionVM>();
        }

        public async Task<List<FundRequisitionVM>> RejectedFundRequistionListForFinance(int userId, int currentPageIndex, int pAGE_SIZE, int UserId, int departmentId)
        {

            var sqlStoredProc = "sp_rejected_fund_requisition_list_for_finance";

            var response = await DapperAdapter.GetFromStoredProcAsync<FundRequisitionVM>
            (
                storedProcedureName: sqlStoredProc,
                parameters: new { @user_Id = userId, @start = currentPageIndex, @rowsperpage = pAGE_SIZE, @UserId = UserId, @DepartmentID = departmentId, @currentstatus = FundRequisitionStatus.Pending },
                dbconnectionString: DefaultConnectionString,
                sqltimeout: DefaultTimeOut,
                dbconnection: _connection,
                dbtransaction: _transaction
            );
            return response.ToList<FundRequisitionVM>();
        }

        public async Task<FundRequisitionVM> GetFundRequisitionHistoryByFundRequisitionId(int fundRequisitionId)
        {
            var sqlStoredProc = "sp_get_fund_requisition_history_by_FundRequisition_id";

            var response = await DapperAdapter.GetFromStoredProcAsync<FundRequisitionVM>
            (
                storedProcedureName: sqlStoredProc,
                parameters: new { FundRequisitonId = fundRequisitionId },
                dbconnectionString: DefaultConnectionString,
                sqltimeout: DefaultTimeOut,
                dbconnection: _connection,
                dbtransaction: _transaction
            );

            return response.FirstOrDefault();
        }

        public async Task<FundRequisitionVM> GetFundRequisitionHistoryByEstimationId(int estimationId , int userId , int departmentId)
        {
            var sqlStoredProc = "sp_get_FundRequisition_details_by_estimation_id";

            var response = await DapperAdapter.GetFromStoredProcAsync<FundRequisitionVM>
            (
                storedProcedureName: sqlStoredProc,
                parameters: new { @estimationId = estimationId , @userId = userId , @departmentId = departmentId },
                dbconnectionString: DefaultConnectionString,
                sqltimeout: DefaultTimeOut,
                dbconnection: _connection,
                dbtransaction: _transaction
            );

            return response.FirstOrDefault();
        } 


        public async Task<List<FundRequisitionDisburseHistory>> getFundRequisitionDisburseHistory(int userId, int departmentId,
            int estimationId)
        {
            var sqlStoredProc = "sp_get_fund_requistion_disburse_history_By_estimate_id_v2";

            var response = await DapperAdapter.GetFromStoredProcAsync<FundRequisitionDisburseHistory>
            (
                storedProcedureName: sqlStoredProc,
                parameters: new { @UserId = userId, @DepartmentID = departmentId, @requisitionType =1, @EstimationId = estimationId },
                dbconnectionString: DefaultConnectionString,
                sqltimeout: DefaultTimeOut,
                dbconnection: _connection,
                dbtransaction: _transaction
            );

            return response.ToList();
        }


        public async Task<List<FundRequisitionDisburseHistory>> getTotalFundRequisitionDisburseHistory(int userId, int departmentId,
            int estimationId)
        {
            var sqlStoredProc = "sp_get_fund_total_requistion_disburse_history_By_estimate_id";

            var response = await DapperAdapter.GetFromStoredProcAsync<FundRequisitionDisburseHistory>
            (
                storedProcedureName: sqlStoredProc,
                parameters: new { @UserId = userId, @DepartmentID = departmentId, @requisitionType = 1, @EstimationId = estimationId },
                dbconnectionString: DefaultConnectionString,
                sqltimeout: DefaultTimeOut,
                dbconnection: _connection,
                dbtransaction: _transaction
            );

            return response.ToList();
        }

        public async Task<int> UpdateFundRequistionStatus(int fundRequistionId, string remarks , int feedback , int userId)
        {
            var sqlStoredProc = "sp_change_fund_requisition_status_and_related_data";

            var response = await DapperAdapter.GetFromStoredProcAsync<int>
            (
                storedProcedureName: sqlStoredProc,
                parameters: new
                {
                    @UserId = userId,
                    @FundRequistionId = fundRequistionId,
                    @FeedBack = feedback,
                    @Remarks = remarks,
                  

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

        public async Task<string> getUserEmailAddressByDepartmentId(int departmentId)
        {
            var sqlStoredProc = "sp_get_Comma_separate_email_by_department";

            var response = await DapperAdapter.GetFromStoredProcAsync<string>
            (
                storedProcedureName: sqlStoredProc,
                parameters: new { @DepartmentId = departmentId },
                dbconnectionString: DefaultConnectionString,
                sqltimeout: DefaultTimeOut,
                dbconnection: _connection,
                dbtransaction: _transaction
            );
            return response.FirstOrDefault().ToString();
        }
        public async Task<List<FundRequisitionDisburseHistory>> getFundRequisitionDisburseHistory_of_other_department(int userId, int departmentId,
            int estimationId)
        {
            var sqlStoredProc = "sp_get_fund_requistion_disburse_history_of_oposite_department_By_estimate_id";

            var response = await DapperAdapter.GetFromStoredProcAsync<FundRequisitionDisburseHistory>
            (
                storedProcedureName: sqlStoredProc,
                parameters: new { @UserId = userId, @DepartmentID = departmentId, @requisitionType = 1, @EstimationId = estimationId },
                dbconnectionString: DefaultConnectionString,
                sqltimeout: DefaultTimeOut,
                dbconnection: _connection,
                dbtransaction: _transaction
            );

            return response.ToList();
        }

        public async Task<List<FundRequisitionVM>> getInCompletedFundRequisitionForCheckFinalSettlement(int userId, int departmentId,
            int estimationId)
        {
            var sqlStoredProc = "sp_get_InCompleted_FundRequisition_For_Check_FinalSettlement";

            var response = await DapperAdapter.GetFromStoredProcAsync<FundRequisitionVM>
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
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMS.Common.Constants;
using AMS.Infrastructure.Adapters;
using AMS.Infrastructure.Configuration.Models;
using AMS.Models.CustomModels;
using AMS.Models.ServiceModels.FundDisburse;

namespace AMS.Repositories.DatabaseRepos.FundDisburseRepo
{
    public  class FundDisburseRepo : BaseSQLRepo, IFundDisburseRepo
    {
        private readonly IDbConnection _connection;
        private readonly IDbTransaction _transaction;

        public FundDisburseRepo(IDbConnection connection, IDbTransaction transaction, ConnectionStringSettings connectionStringsSettings)
            : base(connectionStringsSettings)
        {
            _connection = connection;
            _transaction = transaction;
        }

        public async Task<List<FundDisburseHistory>> GetFundDisburseHistoryByEstimateId(int estimationId)
        {
           
            var sqlStoredProc = "sp_get_fund_disburse_history_by_estimate_Id";

            var response = await DapperAdapter.GetFromStoredProcAsync<FundDisburseHistory>
            (
                storedProcedureName: sqlStoredProc,
                parameters: new { @estimationId },
                dbconnectionString: DefaultConnectionString,
                sqltimeout: DefaultTimeOut,
                dbconnection: _connection,
                dbtransaction: _transaction
            );
            return response.ToList();
        }

        public async Task<int> CreateFundDisburse(Models.DomainModels.FundDisburse request)
        {
            var sqlStoredProc = "sp_create_Fund_Disburse";

            var response = await DapperAdapter.GetFromStoredProcAsync<int>
            (
                storedProcedureName: sqlStoredProc,
                parameters: new
                {
                    @Status = request.Status,
                    @ResponseStatus = FundDisburseUserType.FundDisburseEnd,
                    @NewDisburseAmount = request.DisburseAmount,
                    @FundAvailableDate = request.FundAvailableDate,
                    @RemarksByFinance = request.RemarksByFinance,
                    @FundRequisitionId = request.FundRequisitionId,
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
                throw new Exception("No Fund created");
            }
            return response.FirstOrDefault();
        }

        public async Task<int> FundDisburseReceiveOrRollback(Models.DomainModels.FundDisburse request)
        {
            var sqlStoredProc = "sp_fund_receive_or_RollBack";

            var response = await DapperAdapter.GetFromStoredProcAsync<int>
            (
                storedProcedureName: sqlStoredProc,
                parameters: new
                {
                    @Status = request.Status,
                    @ResponseStatus = FundDisburseUserType.FundReceiverEnd,
                    @ReceiveAmount = request.ReceivedAmount,
                    @FundDisburseId = request.Id,

                    @RemarksByFundReceiver = request.RemarksByFundReceiver,
                   
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
                throw new Exception("No Fund created");
            }
            return response.FirstOrDefault();
        }

        public async Task<List<FundDisburseVM>> PendingRollBackListForReSubmitByFinance(int userId, int currentPageIndex,
            int pAGE_SIZE, int UserId, int departmentId)
        {

            var sqlStoredProc = "sp_fund_disburse_PendingRollBackListForReSubmitByFinance";

            var response = await DapperAdapter.GetFromStoredProcAsync<FundDisburseVM>
            (
                storedProcedureName: sqlStoredProc,
                parameters: new { @user_Id = UserId, @start = currentPageIndex, @rowsperpage = pAGE_SIZE, @UserId = UserId, @DepartmentID = departmentId, @currentstatus = FundDisburseStatus.FundDisburseRollBack },
                dbconnectionString: DefaultConnectionString,
                sqltimeout: DefaultTimeOut,
                dbconnection: _connection,
                dbtransaction: _transaction
            );
            return response.ToList<FundDisburseVM>();
        }

        public async Task<List<FundDisburseVM>> PendingFundDisburseListForAcknowledgement(int userId, int currentPageIndex, int pAGE_SIZE, int UserId, int departmentId)
        {

            var sqlStoredProc = "sp_fund_disburse_list_for_acknowledgement_By_requisitionDepartment";

            var response = await DapperAdapter.GetFromStoredProcAsync<FundDisburseVM>
            (
                storedProcedureName: sqlStoredProc,
                parameters: new { @user_Id = UserId, @start = currentPageIndex, @rowsperpage = pAGE_SIZE, @UserId = UserId, @DepartmentID = departmentId, @currentstatus = FundDisburseStatus.FundDisbursePending },
                dbconnectionString: DefaultConnectionString,
                sqltimeout: DefaultTimeOut,
                dbconnection: _connection,
                dbtransaction: _transaction
            );
            return response.ToList<FundDisburseVM>();
        }

        public async Task<List<FundDisburseVM>> FundDisburseByFinanceWaitingForAcknowledge(int userId, int currentPageIndex, int pAGE_SIZE, int UserId, int departmentId)
        {

            var sqlStoredProc = "sp_fund_disburse_pending_list_for_finance_Department";

            var response = await DapperAdapter.GetFromStoredProcAsync<FundDisburseVM>
            (
                storedProcedureName: sqlStoredProc,
                parameters: new { @user_Id = UserId, @start = currentPageIndex, @rowsperpage = pAGE_SIZE, @UserId = UserId, @DepartmentID = departmentId, @currentstatus = FundDisburseStatus.FundDisbursePending },
                dbconnectionString: DefaultConnectionString,
                sqltimeout: DefaultTimeOut,
                dbconnection: _connection,
                dbtransaction: _transaction
            );
            return response.ToList<FundDisburseVM>();
        }
        //

        public async Task<List<FundDisburseVM>> CompletedDisburseListDepartmentWise(int userId, int currentPageIndex,
            int pAGE_SIZE, int UserId, int departmentId)
        {

            var sqlStoredProc = "sp_fund_disburse_Completed_List_Department_wise";

            var response = await DapperAdapter.GetFromStoredProcAsync<FundDisburseVM>
            (
                storedProcedureName: sqlStoredProc,
                parameters: new { @user_Id = UserId, @start = currentPageIndex, @rowsperpage = pAGE_SIZE, @UserId = UserId, @DepartmentID = departmentId, @currentstatus = FundDisburseStatus.FundDisburseSuccessfullyReceived },
                dbconnectionString: DefaultConnectionString,
                sqltimeout: DefaultTimeOut,
                dbconnection: _connection,
                dbtransaction: _transaction
            );
            return response.ToList<FundDisburseVM>();
        }


        public async Task<FundDisburseVM> GetFundDisburseHistoryByFundDisburseId(int fundDisburseId)
        {
            var sqlStoredProc = "sp_get_fund_disburse_history_by_Fund_Disburse_id";

            var response = await DapperAdapter.GetFromStoredProcAsync<FundDisburseVM>
            (
                storedProcedureName: sqlStoredProc,
                parameters: new { FundDisburseId = fundDisburseId },
                dbconnectionString: DefaultConnectionString,
                sqltimeout: DefaultTimeOut,
                dbconnection: _connection,
                dbtransaction: _transaction
            );

            return response.FirstOrDefault();
        }

        public async Task<int> FundReDisburse(Models.DomainModels.FundDisburse request)
        {
            var sqlStoredProc = "sp_fund_ReDisburse_By_finance";

            var response = await DapperAdapter.GetFromStoredProcAsync<int>
            (
                storedProcedureName: sqlStoredProc,
                parameters: new
                {
                    @Status = request.Status,
                    @ResponseStatus = FundDisburseUserType.FundDisburseEnd,
                    @DisburseAmount = request.DisburseAmount,
                    @FundDisburseId = request.Id,

                    @RemarksByFinance = request.RemarksByFinance,

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
                throw new Exception("No Fund created");
            }
            return response.FirstOrDefault();
        }
    }
}

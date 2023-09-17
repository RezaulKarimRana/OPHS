using AMS.Infrastructure.Adapters;
using AMS.Infrastructure.Configuration.Models;
using AMS.Models.ViewModel;
using AMS.Repositories.DatabaseRepos.AdminSupportRepo.Contracts;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace AMS.Repositories.DatabaseRepos.AdminSupportRepo
{
    public class AdminSupportRepo : BaseSQLRepo, IAdminSupportRepo
    {
        private readonly IDbConnection _connection;
        private readonly IDbTransaction _transaction;

        public AdminSupportRepo(IDbConnection connection, IDbTransaction transaction, ConnectionStringSettings connectionStringsSettings)
            : base(connectionStringsSettings)
        {
            _connection = connection;
            _transaction = transaction;
        }

        public async Task<List<ApproverModificationVM>> GetAllApprover(int moduleId, string requestNo)
        {
            var sqlStoredProc = "sp_approver_get_by_module";

            var response = await DapperAdapter.GetFromStoredProcAsync<ApproverModificationVM>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: new { moduleId, requestNo },
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction
                );

            return response.ToList();
        }
        public async Task<int> UpdateApproverModification(ApproverModificationUpdateModel model)
        {
            try
            {
                var sqlStoredProc = "sp_Approver_Modification";
                var itemsDT = ConvertToDataTable(model.Approvers);

                var request = new
                {
                    ModuleId = model.ModuleId,
                    RequestNo = model.RequestNo,
                    ApproverList = itemsDT.AsTableValuedParameter("ApproverItemsUDT"),
                    P_Key = 0
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
        public async Task<int> UpdateRequestStatus(StatusUpdateModel model)
        {
            try
            {
                var sqlStoredProc = "sp_Update_Request_Status";

                var request = new
                {
                    ModuleId = model.ModuleId,
                    StatusId = model.StatusId,
                    RequestNo = model.RequestNo,
                    P_Key = 0
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
        public async Task<int> UpdateApproverRole(ApproverRoleUpdateModel model)
        {
            try
            {
                var sqlStoredProc = "sp_Update_ApproverRole";

                var request = new
                {
                    ModuleId = model.ModuleId,
                    RequestNo = model.RequestNo,
                    ApproverId = model.ApproverId,
                    RoleId = model.RoleId,
                    P_Key = 0
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
        public async Task<int> UpdateUserDepartment(UserDepartmentUpdateModel model)
        {
            try
            {
                var sqlStoredProc = "sp_Update_User_Department";

                var request = new
                {
                    UserId = model.UserId,
                    DepartmentId = model.DepartmentId,
                    P_Key = 0
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
    }
}

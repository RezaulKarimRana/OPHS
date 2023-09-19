using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AMS.Infrastructure.Adapters;
using AMS.Infrastructure.Configuration.Models;
using AMS.Repositories.DatabaseRepos.DashboardRepo.Contracts;
using AMS.Repositories.DatabaseRepos.DashboardRepo.Models;

namespace AMS.Repositories.DatabaseRepos.DashboardRepo
{
    public class DashboardRepo : BaseSQLRepo, IDashboardRepo
    {
        #region Instance Fields

        private IDbConnection _connection;
        private IDbTransaction _transaction;

        #endregion

        #region Constructor

        public DashboardRepo(IDbConnection connection, IDbTransaction transaction, ConnectionStringSettings connectionStringsSettings)
            : base(connectionStringsSettings)
        {
            _connection = connection;
            _transaction = transaction;
        }

        #endregion

        #region Public Methods

        public async Task<GetDashboardResponse> GetDashboard()
        {
            var sqlStoredProc = "sp_dashboard_get";

            var response = await DapperAdapter.GetFromStoredProcAsync<GetDashboardResponse>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: new { },
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction);

            return response.FirstOrDefault();
        }

        public async Task<GetCountForAllPendingParkingsForNav> GetNavBarCount(int userId)
        {
            var sqlStoredProc = "sp_count_all_pending_parking_for_a_user_get";

            var response = await DapperAdapter.GetFromStoredProcAsync<GetCountForAllPendingParkingsForNav>
                (
                    storedProcedureName: sqlStoredProc,
                    parameters: new { @user_Id = userId },
                    dbconnectionString: DefaultConnectionString,
                    sqltimeout: DefaultTimeOut,
                    dbconnection: _connection,
                    dbtransaction: _transaction);

            return response.FirstOrDefault();
        }

        #endregion
    }
}

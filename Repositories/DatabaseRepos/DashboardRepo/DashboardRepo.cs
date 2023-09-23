using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Adapters;
using Infrastructure.Configuration.Models;
using Repositories.DatabaseRepos.DashboardRepo.Contracts;
using Repositories.DatabaseRepos.DashboardRepo.Models;

namespace Repositories.DatabaseRepos.DashboardRepo
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
            return new GetDashboardResponse();
        }

        public async Task<GetCountForAllPendingParkingsForNav> GetNavBarCount(int userId)
        {
            return new GetCountForAllPendingParkingsForNav();
        }

        #endregion
    }
}

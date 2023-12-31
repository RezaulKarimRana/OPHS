using Infrastructure.Configuration.Models;

namespace Repositories.UnitOfWork
{
    public class BaseUow
    {
        #region Instance Fields

        private readonly ConnectionStringSettings _connectionStrings;

        #endregion

        #region Constructor

        public BaseUow(ConnectionStringSettings connectionStrings)
        {
            _connectionStrings = connectionStrings;
        }

        #endregion

        #region Properties

        protected virtual string DefaultUowConnectionString
        {
            get
            {
                return _connectionStrings.DefaultConnection; // specifies a specific connection string
            }
        }

        #endregion
    }
}

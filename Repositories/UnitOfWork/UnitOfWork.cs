using System;
using System.Data;
using AMS.Infrastructure.Configuration.Models;
using AMS.Repositories.UnitOfWork.Contracts;
using AMS.Repositories.DatabaseRepos.ConfigurationRepo.Contracts;
using AMS.Repositories.DatabaseRepos.SessionRepo.Contracts;
using AMS.Repositories.DatabaseRepos.ConfigurationRepo;
using AMS.Repositories.DatabaseRepos.SessionRepo;
using AMS.Repositories.DatabaseRepos.DashboardRepo;
using AMS.Repositories.DatabaseRepos.DashboardRepo.Contracts;
using AMS.Repositories.DatabaseRepos.UserRepo.Contracts;
using AMS.Repositories.DatabaseRepos.UserRepo;

namespace AMS.Repositories.UnitOfWork
{
    public class UnitOfWork : BaseUow, IUnitOfWork
    {
        #region Instance Fields

        private IDbConnection _connection;
        private IDbTransaction _transaction;
        private IConfigurationRepo _configurationRepo;
        private ISessionRepo _sessionRepo;
        private IDashboardRepo _dashboardRepo;
        private IUserRepo _userRepo;
        private bool _disposed;
        private readonly ConnectionStringSettings _connectionSettings;

        #endregion

        #region Constructor

        public UnitOfWork( IDbConnection dbConnection, ConnectionStringSettings connectionStrings, bool beginTransaction = true) : base(connectionStrings)
        {
            _connectionSettings = connectionStrings;
            _connection = dbConnection;
            _connection.Open();

            if (beginTransaction)
            {
                _transaction = _connection.BeginTransaction();
            }
        }

        #endregion

        #region Properties

        public IConfigurationRepo ConfigurationRepo
        {
            get { return _configurationRepo ??= new ConfigurationRepo(_connection, _transaction, _connectionSettings); }
        }

        public ISessionRepo SessionRepo
        {
            get { return _sessionRepo ??= new SessionRepo(_connection, _transaction, _connectionSettings); }
        }
        public IDashboardRepo DashboardRepo
        {
            get { return _dashboardRepo ??= new DashboardRepo(_connection, _transaction, _connectionSettings); }
        }
        public IUserRepo UserRepo
        {
            get { return _userRepo ??= new UserRepo(_connection, _transaction, _connectionSettings); }
        }
        #endregion

        #region Public Methods

        public bool Commit()
        {
            if (_transaction == null)
            {
                ResetRepositories();
                return true;
            }

            try
            {
                _transaction.Commit();

                return true;
            }
            catch
            {
                _transaction.Rollback();
                throw;
            }
            finally
            {
                _transaction.Dispose();
                _transaction = _connection.BeginTransaction();
                ResetRepositories();
            }
        }

        public void Dispose()
        {
            dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Private Methods

        private void ResetRepositories()
        {
            _configurationRepo = null;
            _sessionRepo = null;
            _dashboardRepo = null;
            _userRepo = null;
        }

        private void dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_transaction != null)
                    {
                        _transaction.Dispose();
                        _transaction = null;
                    }
                    if (_connection != null)
                    {
                        _connection.Dispose();
                        _connection = null;
                    }
                }
                _disposed = true;
            }
        }

        ~UnitOfWork()
        {
            dispose(false);
        }

        #endregion
    }
}
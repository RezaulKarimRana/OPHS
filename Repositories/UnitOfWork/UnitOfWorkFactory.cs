using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;
using Infrastructure.Configuration.Models;
using AMS.Repositories.UnitOfWork.Contracts;

namespace AMS.Repositories.UnitOfWork
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        private readonly ConnectionStringSettings _connectionSettings;

        public UnitOfWorkFactory(IOptions<ConnectionStringSettings> connectionSettings) : base()
        {
            _connectionSettings = connectionSettings.Value;
        }

        public IUnitOfWork GetUnitOfWork(bool beginTransaction = true)
        {
            var connectionString = _connectionSettings.DefaultConnection;
            return new UnitOfWork(new SqlConnection(connectionString), _connectionSettings, beginTransaction);
        }

        public IUnitOfWork GetMySQLUnitOfWork(bool beginTransaction = true)
        {
            var connectionString = _connectionSettings.DefaultConnection;
            return new UnitOfWork(new MySqlConnection(connectionString), _connectionSettings, beginTransaction);
        }
    }
}

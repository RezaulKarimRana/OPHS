using AMS.Infrastructure.Configuration.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;

namespace AMS.Repositories.DatabaseRepos
{
    public class BaseSQLRepo
    {
        private readonly ConnectionStringSettings _connectionStrings;

        public BaseSQLRepo(ConnectionStringSettings connectionStringsSettings)
        {
            _connectionStrings = connectionStringsSettings;
        }

        protected virtual string DefaultConnectionString
        {
            get
            {
                return _connectionStrings.DefaultConnection; // specifies a specific connection string
            }
        }

        protected virtual int DefaultTimeOut
        {
            get
            {
                return _connectionStrings.Timeout;
            }
        }
        public DataTable ConvertToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));

            DataTable table = new DataTable();

            foreach (PropertyDescriptor prop in properties)
            {
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }

            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                {
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                }
                table.Rows.Add(row);
            }
            return table;
        }
    }
}

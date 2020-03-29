using System;
using API.Helpers;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;

namespace API.Data
{
    public interface IDBConnection
    {
        MySqlConnection Connection { get; }

        bool OpenConnection();
        bool CloseConnection();
    }

    public class DBConnection : IDBConnection
    {
        private readonly DBSettings _dbSettings;
        public MySqlConnection Connection { get; private set; }


        public DBConnection(IOptions<AppSettings> appSettings)
        {
            _dbSettings = appSettings.Value.DBSettings;
            Initialize();
        }

        private void Initialize()
        {
            var builder = new MySqlConnectionStringBuilder
            {
                Server = _dbSettings.Server,
                Database = _dbSettings.Database,
                UserID = _dbSettings.UserID,
                Password = _dbSettings.UserID,
                SslMode = MySqlSslMode.Required
            };
            Connection = new MySqlConnection(builder.ConnectionString);
        }

        public bool OpenConnection()
        {
            try
            {
                Connection.Open();
                return true;
            }
            catch (MySqlException)
            {
                return false;
            }
        }

        public bool CloseConnection()
        {
            try
            {
                Connection.Close();
                return true;
            }
            catch (MySqlException)
            {
                return false;
            }
        }
    }
}
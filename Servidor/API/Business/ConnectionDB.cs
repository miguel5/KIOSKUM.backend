using API.Helpers;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;

namespace API.Business
{
    public interface IConnectionDB
    {
        MySqlConnection Connection { get; }
        void OpenConnection();
        void CloseConnection();
    }


    public class ConnectionDB : IConnectionDB
    {
        public MySqlConnection Connection { get; private set; }
        private DBSettings _dbSettings;


        public ConnectionDB(IOptions<AppSettings> appSettings)
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
                Password = _dbSettings.Password,
                SslMode = MySqlSslMode.Required,
            };

            Connection = new MySqlConnection(builder.ConnectionString);
        }


        public void OpenConnection()
        {
            Connection.Open();
        }


        public void CloseConnection()
        {
            Connection.Close();
        }
    }
}
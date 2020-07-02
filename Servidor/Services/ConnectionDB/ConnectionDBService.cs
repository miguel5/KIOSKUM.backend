using System;
using Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;

namespace Services.DBConnection
{
    public class ConnectionDBService : IConnectionDBService, IDisposable
    {
        public MySqlConnection Connection { get; private set; }
        public MySqlConnection Connection2 { get; private set; }
        private ILogger _logger;
        private DBSettings _dbSettings;


        public ConnectionDBService(ILogger<ConnectionDBService> logger, IOptions<AppSettings> appSettings)
        {
            _logger = logger;
            _dbSettings = appSettings.Value.DBSettings;
            Initialize();
        }

        public void OpenConnection()
        {
            _logger.LogDebug("A executar [ConnectionDB -> OpenConnection]");
            Connection.Open();
            _logger.LogDebug("Conexão aberta");
        }

        public void CloseConnection()
        {
            _logger.LogDebug("A executar [ConnectionDB -> CloseConnection]");
            Connection.Close();
            _logger.LogDebug("Conexão fechada");
        }

        public void OpenConnection2()
        {
            _logger.LogDebug("A executar [ConnectionDB -> OpenConnection2]");
            Connection2.Open();
            _logger.LogDebug("Conexão 2 aberta");
        }

        public void CloseConnection2()
        {
            _logger.LogDebug("A executar [ConnectionDB -> CloseConnection2]");
            Connection2.Close();
            _logger.LogDebug("Conexão 2 fechada");
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        ~ConnectionDBService() => Dispose(false);


        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Connection != null) { Connection.Dispose(); Connection = null; }
                if (Connection2 != null) { Connection2.Dispose(); Connection2 = null; }
            }
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
            Connection2 = new MySqlConnection(builder.ConnectionString);
        }
    }
}
using API.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;

namespace API.Services
{
    public class ConnectionDBService : IConnectionDBService
    {
        public MySqlConnection Connection { get; private set; }
        private ILogger _logger;
        private DBSettings _dbSettings;


        public ConnectionDBService(ILogger<ConnectionDBService> logger, IOptions<AppSettings> appSettings)
        {
            _logger = logger;
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
            _logger.LogDebug("A executar [ConnectionDB -> OpenConnection]");
            Connection.Open();
            _logger.LogDebug("Conexão aberta");
        }


        public void CloseConnection()
        {
            _logger.LogDebug("A executar [ConnectionDB -> OpenConnection]");
            Connection.Close();
            _logger.LogDebug("Conexão fechada");
        }
    }
}
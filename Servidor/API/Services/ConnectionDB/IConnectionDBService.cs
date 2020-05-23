using MySql.Data.MySqlClient;

namespace API.Services.DBConnection
{
    public interface IConnectionDBService
    {
        MySqlConnection Connection { get; }
        void OpenConnection();
        void CloseConnection();
    }
}

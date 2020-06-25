using MySql.Data.MySqlClient;

namespace Services.DBConnection
{
    public interface IConnectionDBService
    {
        MySqlConnection Connection { get; }
        void OpenConnection();
        void CloseConnection();
    }
}

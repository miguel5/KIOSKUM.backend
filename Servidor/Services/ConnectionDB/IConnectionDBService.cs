using MySql.Data.MySqlClient;

namespace Services.DBConnection
{
    public interface IConnectionDBService
    {
        MySqlConnection Connection { get; }
        MySqlConnection Connection2 { get; }
        void OpenConnection();
        void CloseConnection();
        void OpenConnection2();
        void CloseConnection2();
    }
}
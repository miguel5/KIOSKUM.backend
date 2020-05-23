using MySql.Data.MySqlClient;

namespace API.Services
{
    public interface IConnectionDBService
    {
        MySqlConnection Connection { get; }
        void OpenConnection();
        void CloseConnection();
    }
}

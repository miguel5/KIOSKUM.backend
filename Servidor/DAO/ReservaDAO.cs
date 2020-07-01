using System.Collections.Generic;
using DAO.Interfaces;
using Entities;

namespace DAO
{
    public class ReservaDAO : IReservaDAO
    {
        public ReservaDAO()
        {
        }

        public void EditarReserva(Reserva reserva)
        {
            throw new System.NotImplementedException();
        }

        public bool ExisteReserva(int idReserva)
        {
            _logger.LogDebug("A executar [ReservaDAO -> ExisteReserva]");
            try
            {
                _connectionDBService.OpenConnection();
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = _connectionDBService.Connection;

                    cmd.CommandText = "existe_reserva";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("?id", idReserva);
                    cmd.Parameters["?id"].Direction = ParameterDirection.Input;

                    object val = cmd.ExecuteScalar();

                    return Convert.ToBoolean(val);
                }
            }
            catch { throw; }
            finally
            {
                _connectionDBService.CloseConnection();
            }
        }

        public Reserva GetReserva(int idReserva)
        {
            throw new System.NotImplementedException();
        }

        public List<Reserva> GetReservasEstado(int estado)
        {
            throw new System.NotImplementedException();
        }

        public void RegistarReserva(Reserva reserva)
        {
            throw new System.NotImplementedException();
        }
    }
}

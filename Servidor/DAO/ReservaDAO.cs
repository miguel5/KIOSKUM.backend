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
            throw new System.NotImplementedException();
        }

        public Reserva GetReserva(int idReserva)
        {
            throw new System.NotImplementedException();
        }

        public IList<Reserva> GetReservasEstado(int estado)
        {
            IList<Reserva> lista = new List<Reserva>();
            for(int i = 0; i < 200 ; i++)
            {
                lista.Add(new Reserva() { Estado = EstadosReservaEnum.Aceite});
            }
            return lista;
        }

        public void RegistarReserva(Reserva reserva)
        {
            throw new System.NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using Entities;

namespace DAO.Interfaces
{
    public interface IReservaDAO
    {
        void RegistarReserva(Reserva reserva);
        bool ExisteReserva(int idReserva);
        Reserva GetReserva(int idReserva);
        void EditarReserva(Reserva reserva);
        IList<Reserva> GetReservasEstado(int estado);
    }
}

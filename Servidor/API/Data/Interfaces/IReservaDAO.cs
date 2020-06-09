using System;
using System.Collections.Generic;
using API.Entities;

namespace API.Data.Interfaces
{
    public interface IReservaDAO
    {
        void RegistarReserva(Reserva reserva);
        bool ExisteReserva(int idReserva);
        Reserva GetReserva(int idReserva);
        void EditarReserva(Reserva reserva);
        List<Reserva> GetReservasEstado(int estado);
    }
}

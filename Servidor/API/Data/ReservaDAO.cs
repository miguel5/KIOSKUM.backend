using System;
using API.Entities;

namespace API.Data
{
    public class ReservaDAO
    {
        public ReservaDAO()
        {
        }

        internal void RegistarReserva(Reserva reserva)
        {
            throw new NotImplementedException();
        }

        internal void AlterarEstadoReserva(int idReserva, EstadosReservaEnum novoEstado, int idFuncionario)
        {
            throw new NotImplementedException();
        }

        internal void RegistarPagamento(int idReserva, DateTime horaPagamento)
        {
            throw new NotImplementedException();
        }
    }
}

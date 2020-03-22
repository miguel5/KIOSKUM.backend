using System;
using System.Collections.Generic;
using API.Data;
using API.Models;

namespace API.Business
{
    public class Reservas
    {
        private readonly ReservaDAO reservaDAO;

        public Reservas()
        {
            reservaDAO = new ReservaDAO();
        }

        public void PedidoReserva(int IdCliente, IList<Tuple<int, int, string>> Items, double Preco, DateTime HoraEntrega)
        {
            if(Items == null)
            {
                throw new ArgumentNullException("Items", "Parametro não pode ser nulo");
            }

            Reserva reserva = new Reserva(IdCliente, Items, Preco, HoraEntrega);

            reservaDAO.RegistarReserva(reserva);
        }


        public void AlterarReserva(int IdReserva, EstadosReservaEnum NovoEstado, int IdFuncionario)
        {
            reservaDAO.AlterarEstadoReserva(IdReserva, NovoEstado, IdFuncionario);
        }


        public void RegistaPagamento(int IdReserva, DateTime HoraPagamento)
        {
            reservaDAO.RegistarPagamento(IdReserva, HoraPagamento);
        }



    }
}

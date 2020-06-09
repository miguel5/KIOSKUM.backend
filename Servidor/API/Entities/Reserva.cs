using System;
using System.Collections.Generic;

namespace API.Entities
{
    public class Reserva
    {
        public int IdReserva { get; set; }
        public int IdCliente { get; set; }
        public int IdFuncionarioDecide { get; set; }
        public int IdFuncionarioEntrega { get; set; }
        public IList<Item> Items { get; set; } 
        public EstadosReservaEnum Estado { get; set; }
        public double Preco { get; set; }
        public DateTime HoraEntrega { get; set; }
        public DateTime HoraPagamento { get; set; }
    }
}
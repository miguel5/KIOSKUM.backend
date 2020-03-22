using System;
using System.Collections.Generic;
using System.Text;

namespace API.Entities
{
    public class Reserva
    {
        public int IdReserva { get; set; }
        public int IdCliente { get; set; }
        public Tuple<int, int> IdFuncionarios { get; private set; }
        public IList<Tuple<int, int, string>> Items { get; set; } //(idProduto,Quantidade,Observações)
        public EstadosReservaEnum Estado { get; set; }
        public double Preco { get; set; }
        public DateTime HoraEntrega { get; set; }
        public DateTime HoraPagamento { get; set; }
    }
}

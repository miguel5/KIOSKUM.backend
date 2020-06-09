using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using API.Entities;

namespace API.ViewModels.ReservaDTOs
{
    public class ReservaViewDTO
    {
        [Required]
        public int IdReserva { get; set; }

        [Required]
        public int IdCliente { get; set; }

        [Required]
        public int IdFuncionarioDecide { get; set; }

        [Required]
        public int IdFuncionarioEntrega { get; set; }

        [Required]
        public IList<ItemViewDTO> Itens { get; set; }

        [Required]
        public EstadosReservaEnum Estado { get; set; }

        [Required]
        public double Preco { get; set; }

        [Required]
        public DateTime HoraEntrega { get; set; }

        [Required]
        public DateTime HoraPagamento { get; set; }
    }
}

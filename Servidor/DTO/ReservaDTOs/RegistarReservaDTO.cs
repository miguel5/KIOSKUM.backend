using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Entities;

namespace DTO.ReservaDTOs
{
    public class RegistarReservaDTO
    { 
        [Required]
        public IList<Item> Itens { get; set; }

        [Required]
        public DateTime HoraEntrega { get; set; }
    }
}
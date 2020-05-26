using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using API.Entities;

namespace API.ViewModels.ReservaDTOs
{
    public class RegistarReservaDTO
    { 
        [Required]
        public IList<Item> Itens { get; set; }

        [Required]
        public DateTime HoraEntrega { get; set; }
    }
}

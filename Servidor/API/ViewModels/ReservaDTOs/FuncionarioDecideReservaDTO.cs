using System;
using System.ComponentModel.DataAnnotations;

namespace API.ViewModels.ReservaDTOs
{
    public class FuncionarioDecideReservaDTO
    {
        [Required]
        public int IdReserva { get; set; }

        [Required]
        public int NumFuncionario { get; set; }
    }
}

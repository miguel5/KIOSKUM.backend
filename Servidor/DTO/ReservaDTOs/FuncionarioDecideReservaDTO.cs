using System;
using System.ComponentModel.DataAnnotations;

namespace DTO.ReservaDTOs
{
    public class FuncionarioDecideReservaDTO
    {
        [Required]
        public int IdReserva { get; set; }

        [Required]
        public int NumFuncionario { get; set; }
    }
}

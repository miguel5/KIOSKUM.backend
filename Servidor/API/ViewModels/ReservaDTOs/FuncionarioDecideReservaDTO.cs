using System;
using System.ComponentModel.DataAnnotations;

namespace API.ViewModels.ReservaDTOs
{
    public class FuncionarioDecideReservaDTO
    {
        [Required]
        public int IdReserva;

        [Required]
        public int NumFuncionario;

        [Required]
        public bool Decisao;
    }
}

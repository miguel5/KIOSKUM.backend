using System.ComponentModel.DataAnnotations;

namespace API.ViewModels.ReservaDTOs
{
    public class EntregarReservaDTO
    {
        [Required]
        public int NumFuncionario { get; set; }

        [Required]
        public int IdReserva { get; set; }
    }
}

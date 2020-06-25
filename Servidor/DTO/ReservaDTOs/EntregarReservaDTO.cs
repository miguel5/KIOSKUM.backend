using System.ComponentModel.DataAnnotations;

namespace DTO.ReservaDTOs
{
    public class EntregarReservaDTO
    {
        [Required]
        public int NumFuncionario { get; set; }

        [Required]
        public int IdReserva { get; set; }
    }
}

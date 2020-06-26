using System.ComponentModel.DataAnnotations;

namespace DTO.TrabalhadorDTOs
{
    public class EditarTrabalhadorDTO
    {
        [Required]
        public string Nome { get; set; }

        [Required]
        public int NumFuncionario { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string NovaPassword { get; set; }
    }
}
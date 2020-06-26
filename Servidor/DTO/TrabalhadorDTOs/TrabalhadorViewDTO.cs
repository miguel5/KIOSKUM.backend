using System.ComponentModel.DataAnnotations;

namespace DTO.TrabalhadorDTOs
{
    public class TrabalhadorViewDTO
    {
        [Required]
        public string Nome { get; set; }

        [Required]
        public int NumFuncionario { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;

namespace DTO.TrabalhadorDTOs
{
    public class AutenticacaoTrabalhadorDTO
    {
        [Required]
        public int NumFuncionario { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
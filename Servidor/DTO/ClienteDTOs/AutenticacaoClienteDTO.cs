using System.ComponentModel.DataAnnotations;

namespace DTO.ClienteDTOs
{
    public class AutenticacaoClienteDTO
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
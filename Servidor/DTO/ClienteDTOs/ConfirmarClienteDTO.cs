using System.ComponentModel.DataAnnotations;

namespace DTO.ClienteDTOs
{
    public class ConfirmarClienteDTO
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Codigo { get; set; }
    }
}

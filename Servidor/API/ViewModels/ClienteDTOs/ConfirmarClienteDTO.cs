using System.ComponentModel.DataAnnotations;

namespace API.ViewModels.ClienteDTOs
{
    public class ConfirmarClienteDTO
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Codigo { get; set; }
    }
}

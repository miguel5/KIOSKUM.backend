using System.ComponentModel.DataAnnotations;

namespace API.ViewModels
{
    public class ConfirmarClienteDTO
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Codigo { get; set; }
    }
}

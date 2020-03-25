using System.ComponentModel.DataAnnotations;

namespace API.ViewModels
{
    public class AutenticacaoDTO
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;

namespace API.ViewModels.ClienteDTOs
{
    public class ClienteDTO
    {
        [Required]
        public string Nome { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public int NumTelemovel { get; set; }
    }
}
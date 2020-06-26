using System.ComponentModel.DataAnnotations;

namespace DTO.ClienteDTOs
{
    public class ClienteViewDTO
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
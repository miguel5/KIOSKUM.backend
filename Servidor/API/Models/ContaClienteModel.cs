using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class ContaClienteModel
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

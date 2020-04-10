using System.ComponentModel.DataAnnotations;

namespace API.ViewModels
{
    public class AdministradorDTO
    {
        [Required]
        public string Nome { get; set; }

        [Required]
        public int NumFuncionario { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
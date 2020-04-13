using System.ComponentModel.DataAnnotations;

namespace API.ViewModels
{
    public class EditarAdministradorDTO
    {
        [Required]
        public string Nome { get; set; }

        [Required]
        public int NumFuncionario { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string PasswordAtual { get; set; }

        [Required]
        public string NovaPassword { get; set; }
    }
}

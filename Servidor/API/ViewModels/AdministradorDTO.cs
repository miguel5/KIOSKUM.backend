using System.ComponentModel.DataAnnotations;

namespace API.ViewModels
{
    public class AdministradorDTO : FuncionarioDTO
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
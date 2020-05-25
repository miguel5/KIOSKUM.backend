using System.ComponentModel.DataAnnotations;
using API.ViewModels.FuncionarioDTOs;

namespace API.ViewModels.AdministradorDTOs
{
    public class EditarAdministradorDTO : FuncionarioViewDTO
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string NovaPassword { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using API.ViewModels.FuncionarioDTOs;

namespace API.ViewModels
{
    public class AdministradorDTO : FuncionarioViewDTO
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
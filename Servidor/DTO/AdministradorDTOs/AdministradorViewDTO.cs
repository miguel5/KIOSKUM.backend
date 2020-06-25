using System.ComponentModel.DataAnnotations;
using DTO.FuncionarioDTOs;

namespace DTO.AdministradorDTOs
{
    public class AdministradorViewDTO : FuncionarioViewDTO
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace DTO.FuncionarioDTOs
{
    public class FuncionarioViewDTO
    {
        [Required]
        public string Nome { get; set; }

        [Required]
        public int NumFuncionario { get; set; }
    }
}

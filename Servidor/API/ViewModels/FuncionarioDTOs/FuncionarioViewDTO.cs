using System.ComponentModel.DataAnnotations;

namespace API.ViewModels.FuncionarioDTOs
{
    public class FuncionarioViewDTO
    {
        [Required]
        public string Nome { get; set; }

        [Required]
        public int NumFuncionario { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;

namespace API.ViewModels
{
    public class FuncionarioDTO
    {
        [Required]
        public string Nome { get; set; }

        [Required]
        public int NumFuncionario { get; set; }
    }
}

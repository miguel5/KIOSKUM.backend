using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace DTO.CategoriaDTOs
{
    public class RegistarCategoriaDTO
    {
        [Required]
        public string Nome { get; set; }

        [Required]
        [DataType(DataType.Upload)]
        public IFormFile File { get; set; }
    }
}
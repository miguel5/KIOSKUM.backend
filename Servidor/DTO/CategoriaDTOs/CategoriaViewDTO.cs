using System;
using System.ComponentModel.DataAnnotations;

namespace DTO.CategoriaDTOs
{
    public class CategoriaViewDTO
    {
        [Required]
        public int IdCategoria { get; set; }

        [Required]
        public string Nome { get; set; }

        [Required]
        public Uri Url { get; set; }
    }
}

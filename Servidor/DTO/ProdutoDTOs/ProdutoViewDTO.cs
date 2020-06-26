using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DTO.ProdutoDTOs
{
    public class ProdutoViewDTO
    {
        [Required]
        public string Nome { get; set; }

        [Required]
        public int IdCategoria { get; set; }

        [Required]
        public double Preco { get; set; }

        [Required]
        public IList<string> Ingredientes { get; set; }

        [Required]
        public IList<string> Alergenios { get; set; }

        [Required]
        public Uri Url { get; set; }
    }
}
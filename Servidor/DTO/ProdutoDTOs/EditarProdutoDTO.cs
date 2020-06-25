using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace DTO.ProdutoDTOs
{
    public class EditarProdutoDTO
    {
        [Required]
        public int IdProduto { get; set; }
      
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
        [DataType(DataType.Upload)]
        public IFormFile File { get; set; }

    }
}

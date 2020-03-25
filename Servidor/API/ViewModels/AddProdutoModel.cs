using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class AddProdutoModel
    {
        [Required]
        public string Nome { get; set; }

        [Required]
        public string NomeCategoria { get; set; }

        [Required]
        public double Preco { get; set; }

        [Required]
        public IList<string> Ingredientes { get; set; }

        [Required]
        public IList<string> Alergenios { get; set; }
    }
}

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Policy;

namespace API.ViewModels.ProdutoDTOs
{
    public class RegistarProdutoDTO
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
        public Url Url { get; set; }
    }
}

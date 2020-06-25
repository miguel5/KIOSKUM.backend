using System.Collections.Generic;

namespace Entities
{
    public class Produto
    {
        public int IdProduto { get; set; }
        public string Nome { get; set; }
        public int IdCategoria { get; set; }
        public double Preco { get; set; }
        public IList<string> Ingredientes { get; set; }
        public IList<string> Alergenios { get; set; }
        public string ExtensaoImagem { get; set; }
    }
}
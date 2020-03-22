using System;
using System.Collections.Generic;
using System.Text;

namespace API.ViewModel
{
    public class ProdutoModelView
    {
        public int IdProduto { get; set; }
        public string Nome { get; set; }
        public string Categoria { get; set; }
        public double Preco { get; set; }
        public IList<string> Ingredientes { get; set; }
        public IList<string> Alergenios { get; set; }
        public string URLProduto { get; set; }

        public ProdutoModelView() { }

        public ProdutoModelView(int IdProduto, string Nome, string Categoria, double Preco, IList<string> Ingredientes, IList<string> Alergenios, string URLProduto)
        {
            this.IdProduto = IdProduto;
            this.Nome = Nome;
            this.Categoria = Categoria;
            this.Preco = Preco;
            this.Ingredientes = Ingredientes;
            this.Alergenios = Alergenios;
            this.URLProduto = URLProduto;
        }

        

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Produto\n");
            sb.Append("- ID : " + IdProduto + "\n");
            sb.Append("- Nome : " + Nome + "\n");
            sb.Append("- Categoria : " + Categoria + "\n");
            sb.Append("- Preco : " + Preco + "\n");
            sb.Append("- Ingredientes: ");
            foreach (var ingrediente in Ingredientes)
            {
                sb.Append(ingrediente + "; ");
            }
            sb.Append("\n- Alergénios: ");
            foreach (var alergenio in Alergenios)
            {
                sb.Append(alergenio + "; ");
            }
            sb.Append("- URLProduto: " + URLProduto + "\n");
            return sb.ToString();
        }
    }
}

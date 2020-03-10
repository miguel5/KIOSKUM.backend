using System;
using System.Collections.Generic;
using System.Text;

namespace API
{
    public class Produto
    {
        public int IdProduto { get; }
        public string Nome { get; set; }
        public string Categoria { get; set; }
        public double Preco { get; set; }
        public IList<string> Ingredientes { get; set; }
        public IList<string> Alergenios { get; set; }
        public string PathImagem { get; set; }


        private static int id = 0;

        public Produto(string Nome, string Categoria, double Preco, IList<string> Ingredientes, IList<string> Alergenios, string PathImagem)
        {
            this.IdProduto = id++;
            this.Nome = Nome;
            this.Categoria = Categoria;
            this.Preco = Preco;
            this.Ingredientes = Ingredientes;
            this.Alergenios = Alergenios;
            this.PathImagem = PathImagem;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(Object obj)
        {
            if (obj == this)
            {
                return true;
            }
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            Produto produto = (Produto)obj;
            return IdProduto == produto.IdProduto;
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
            sb.Append("\n- Path Imagem: " + PathImagem + "\n");
            return sb.ToString();
        }

    }
}

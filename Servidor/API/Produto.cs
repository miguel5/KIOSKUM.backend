using System;
using System.Text;

namespace API
{
    public class Produto
    {
        public int IdProduto { get; set; }
        public string Nome { get; set; }
        public string Categoria { get; set; }
        public double Preco { get; set; }

        private static int id = 0;

        public Produto(string Nome, string Categoria, double vPreco)
        {
            this.IdProduto = id++;
            this.Nome = Nome;
            this.Categoria = Categoria;
            this.Preco = Preco;
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
            return sb.ToString();
        }
    }
}

using System;
using System.Text;

namespace API.Models
{
    public class Categoria
    {
        public int IdCategoria { get; }
        public string Nome { get; set; }

        public Categoria(int IdCategoria, string Nome)
        {
            this.IdCategoria = IdCategoria;
            this.Nome = Nome;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = 29 * hash + IdCategoria;
                hash = 29 * hash + (Nome == null ? 0 : Nome.GetHashCode());
                return hash;
            }
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
            Categoria categoria = (Categoria)obj;
            return IdCategoria == categoria.IdCategoria;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Produto\n");
            sb.Append("- ID : " + IdCategoria + "\n");
            sb.Append("- Nome : " + Nome + "\n");
            return sb.ToString();
        }
    }
}

using System;
using System.Text;

namespace API.Models
{
    public abstract class Utilizador
    {
        public int IdUtilizador { get; }
        public string Nome { get; set; }
        public string PathImagem { get; set; }

        private static int id = 0;

        public Utilizador(string Nome, string PathImagem)
        {
            this.IdUtilizador = id++;
            this.Nome = Nome;
            this.PathImagem = PathImagem;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 5;
                hash = 41 * hash + IdUtilizador;
                hash = 41 * hash + (Nome == null ? 0 : Nome.GetHashCode());
                hash = 41 * hash + (PathImagem == null ? 0 : PathImagem.GetHashCode());
                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == this)
            {
                return true;
            }
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            Utilizador utilizador = (Utilizador)obj;
            return IdUtilizador == utilizador.IdUtilizador;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("- ID : " + IdUtilizador + "\n");
            sb.Append("- Nome : " + Nome + "\n");
            sb.Append("- Path Imagem: " + PathImagem + "\n");
            return sb.ToString();
        }
    }
}

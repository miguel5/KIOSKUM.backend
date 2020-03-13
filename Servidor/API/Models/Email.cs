using System;
using System.Text;

namespace API.Models
{
    public class Email
    {
        public string Assunto { get; }
        public string Conteudo { get; }

        public Email(string Assunto, string Conteudo)
        {
            this.Assunto = Assunto;
            this.Conteudo = Conteudo;
        }


        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 13;
                hash = 59 * hash + (Assunto == null ? 0 : Assunto.GetHashCode());
                hash = 59 * hash + (Conteudo == null ? 0 : Conteudo.GetHashCode());
                return hash;
            }
        }


        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Email\n");
            sb.Append("- Assunto : " + Assunto + "\n");
            sb.Append("- Conteudo : " + Conteudo + "\n");
            return sb.ToString();
        }
    }
}
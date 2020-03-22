    using System;
using System.Text;

namespace API.Models
{
    public class Funcionario
    {
        public int IdFuncionario { get; set; }
        public string Nome { get; set; }
        public int NumFuncionario { get; set; }


        public Funcionario() { }


        public Funcionario(string Nome, int NumFuncionario)
        {
            this.Nome = Nome;
            this.NumFuncionario = NumFuncionario;
        }


        public Funcionario(int IdFuncionario, string Nome, int NumFuncionario)
        {
            this.IdFuncionario = IdFuncionario;
            this.Nome = Nome;
            this.NumFuncionario = NumFuncionario;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 5;
                hash = 41 * hash + IdFuncionario;
                hash = 41 * hash + (Nome == null ? 0 : Nome.GetHashCode());
                hash = 41 * hash + NumFuncionario;
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

            Funcionario funcionario = (Funcionario)obj;

            return IdFuncionario == funcionario.IdFuncionario;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Funcionário\n");
            sb.Append("- IdFuncionario: " + IdFuncionario + "\n");
            sb.Append("- Nome: " + Nome + "\n");
            sb.Append("- Numero de Funcionario: " + NumFuncionario + "\n");
            return sb.ToString();
        }
    }
}

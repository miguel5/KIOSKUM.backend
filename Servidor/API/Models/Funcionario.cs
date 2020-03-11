    using System;
using System.Text;

namespace API.Models
{
    public class Funcionario : Utilizador
    {
        public int NumFuncionario { get; }

        public Funcionario(int IdUtilizador, string Nome, int NumFuncionario) : base(IdUtilizador, Nome)
        {
            this.NumFuncionario = NumFuncionario;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 13;
                hash = 53 * hash + NumFuncionario;
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
            return base.Equals(obj);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Funcionário\n");
            sb.Append(base.ToString());
            sb.Append("Numero de Funcionario: " + NumFuncionario + "\n");
            return sb.ToString();
        }
    }
}

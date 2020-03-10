    using System;
using System.Text;

namespace API.Models
{
    public class Funcionario : Utilizador
    {
        public int NumFuncionario { get; }

        public Funcionario(string Nome, string PathImagem, int NumFuncionario) : base(Nome, PathImagem)
        {
            this.NumFuncionario = NumFuncionario;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
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

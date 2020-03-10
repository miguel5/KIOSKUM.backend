using System;
using System.Text;

namespace API.Models
{
    public class Administrador : Utilizador
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public int NumFuncionario { get; }

        public Administrador(string Nome, string PathImagem, string Email, string Password, int NumFuncionario) : base(Nome, PathImagem)
        {
            this.Email = Email;
            this.Password = Password;
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
            sb.Append("Administrador\n");
            sb.Append(base.ToString());
            sb.Append("- Email: " + Email + "\n");
            sb.Append("Numero de Funcionario: " + NumFuncionario + "\n");
            return sb.ToString();
        }

    }
}

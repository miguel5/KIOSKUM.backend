using System;
using System.Text;

namespace API.Models
{
    public class Cliente : Utilizador
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public int NumTelemovel { get; set; }

        public Cliente(int IdUtilizador, string Nome, string Email, string Password, int NumTelemovel) : base(IdUtilizador, Nome)
        {
            this.Email = Email;
            this.Password = Password;
            this.NumTelemovel = NumTelemovel;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 11;
                hash = 47 * hash + (Email == null ? 0 : Email.GetHashCode());
                hash = 47 * hash + (Password == null ? 0 : Password.GetHashCode());
                hash = 47 * hash + NumTelemovel;
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
            sb.Append("Cliente\n");
            sb.Append(base.ToString());
            sb.Append("- Email: " + Email + "\n");
            sb.Append("- Telemóvel: " + NumTelemovel + "\n");
            return sb.ToString();
        }
    }
}

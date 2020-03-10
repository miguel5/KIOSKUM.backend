using System;
using System.Text;

namespace API.Models
{
    public class Cliente : Utilizador
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public int NumTelemovel { get; set; }

        public Cliente(string Nome, string PathImagem, string Email, string Password, int NumTelemovel) : base(Nome, PathImagem)
        {
            this.Email = Email;
            this.Password = Password;
            this.NumTelemovel = NumTelemovel;
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
            sb.Append("Cliente\n");
            sb.Append(base.ToString());
            sb.Append("- Email: " + Email + "\n");
            sb.Append("- Telemóvel: " + NumTelemovel + "\n");
            return sb.ToString();
        }
    }
}

using System;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace API.Models
{
    public class Administrador : Utilizador
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public int NumFuncionario { get; }

        public Administrador(int IdUtilizador, string Nome, string Email, string Password, int NumFuncionario) : base(IdUtilizador, Nome)
        {
            this.Email = Email;
            this.Password = HashPassword(Password);
            this.NumFuncionario = NumFuncionario;
        }

        private string HashPassword(string password)
        {
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: new byte[0],
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
        }


        public override int GetHashCode()
        { 
            unchecked
            {
                int hash = 7;
                hash = 43 * hash + (Email == null ? 0 : Email.GetHashCode());
                hash = 43 * hash + (Password == null ? 0 : Password.GetHashCode());
                hash = 43 * hash + NumFuncionario;
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
            sb.Append("Administrador\n");
            sb.Append(base.ToString());
            sb.Append("- Email: " + Email + "\n");
            sb.Append("Numero de Funcionario: " + NumFuncionario + "\n");
            return sb.ToString();
        }

    }
}

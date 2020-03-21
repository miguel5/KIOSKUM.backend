using System;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace API.Models
{
    public class Cliente 
    {
        public int IdCliente { get; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Password { get; private set; }
        public int NumTelemovel { get; set; }


        public Cliente() { }

        public Cliente(int IdCliente, string Nome, string Email, string Password, int NumTelemovel)
        {
            this.IdCliente = IdCliente;
            this.Nome = Nome;
            this.Email = Email;
            SetPassword(Password);
            this.NumTelemovel = NumTelemovel;
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

        public void SetPassword(string password)
        {
            Password = HashPassword(password);
        }

        public bool ComparaPasswords(string password)
        {
            string hash = HashPassword(password);
            return hash.Equals(Password);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 11;
                hash = 47 * hash + IdCliente;
                hash = 47 * hash + (Nome == null ? 0 : Nome.GetHashCode());
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
            sb.Append("- IdCliente: " + IdCliente + "\n");
            sb.Append("- Nome : " + Nome + "\n");
            sb.Append("- Email: " + Email + "\n");
            sb.Append("- Número de Telemóvel: " + NumTelemovel + "\n");
            return sb.ToString();
        }
    }
}

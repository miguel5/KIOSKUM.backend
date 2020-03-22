using System;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace API.Entities
{
    public class Cliente 
    {
        public int IdCliente { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Password { get; private set; }
        public int NumTelemovel { get; set; }


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
    }
}

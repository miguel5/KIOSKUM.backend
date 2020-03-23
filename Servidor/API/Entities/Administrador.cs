using System;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace API.Entities
{
   public class Administrador : Funcionario
    {
        public string Email { get; set; }
        public string Password
        {
            get => Password;
            set => Password = HashPassword(value);
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
    }
}

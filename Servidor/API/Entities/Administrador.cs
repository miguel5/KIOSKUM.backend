using System;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace API.Entities
{
   public class Administrador : Funcionario
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}

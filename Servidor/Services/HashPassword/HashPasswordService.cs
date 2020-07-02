using System;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Logging;

namespace Services.HashPassword
{
    public class HashPasswordService : IHashPasswordService
    {
        private readonly ILogger _logger;

        public HashPasswordService(ILogger<HashPasswordService> logger)
        {
            _logger = logger;
        }


        public string HashPassword(string password)
        {
            _logger.LogDebug("A executar [HashPasswordService -> HashPassword]");
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: new byte[0],
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
        }
    }
}
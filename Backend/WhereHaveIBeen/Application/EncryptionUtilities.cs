using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application
{
    public static class EncryptionUtilities
    {
        private const string Salt = "DG35!HJ%&U%#%^^$TG#$%^G";

        public static string Encrypt(this string source, string salt = Salt)
        {
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: source,
            salt: Encoding.UTF8.GetBytes(salt),
            prf: KeyDerivationPrf.HMACSHA1,
            iterationCount: 10000,
            numBytesRequested: 256 / 8));

            return hashed;    
        }
    }
}

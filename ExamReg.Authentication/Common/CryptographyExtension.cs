using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ExamReg.Authentication.Common
{
    public static class CryptographyExtentions
    {
        /// <summary>
        /// Băm password với salt tự generate ngẫu nhiên. Salt được trả về.
        /// </summary>
        /// <param name="password">Password plain text</param>
        /// <param name="salt">Salt tự generate ngẫu nhiên, trả về</param>
        /// <returns>Password sau khi băm</returns>
        //public static string HashHMACSHA256(this string password, out string salt)
        //{
        //    var randomSaltBytes = GenerateSalt();
        //    salt = Convert.ToBase64String(randomSaltBytes);
        //    return Hash(password, randomSaltBytes);
        //}

        /// <summary>
        /// Băm password với salt có sẵn.
        /// </summary>
        /// <param name="password">Password hashed</param>
        /// <param name="salt">Salt có sẵn</param>
        /// <returns>Password sau khi băm</returns>
        public static string HashHMACSHA256(this string password, string salt)
        {
            var realSalt = string.IsNullOrEmpty(salt) ? string.Empty : salt;
            return Hash(password, Encoding.ASCII.GetBytes(realSalt));
        }

        public static byte[] GenerateSalt()
        {
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }
        private static string Hash(string password, byte[] salt)
        {
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
        }
    }
}

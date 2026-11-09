using System;
using System.Security.Cryptography;
using System.Text;

namespace _25DH110740_MyStore.Helpers
{
    public static class PasswordHelper
    {
        // Định dạng: salt(base64):hash(base64) - SHA256(salt+password)
        public static string Hash(string password)
        {
            var saltBytes = new byte[16];
            using (var rng = new RNGCryptoServiceProvider()) rng.GetBytes(saltBytes);
            var salt = Convert.ToBase64String(saltBytes);
            return salt + ":" + Compute(salt, password);
        }

        public static bool Verify(string password, string stored)
        {
            if (string.IsNullOrEmpty(stored) || password == null) return false;
            var parts = stored.Split(':');
            if (parts.Length != 2) return false;
            return Compute(parts[0], password) == parts[1];
        }

        private static string Compute(string salt, string password)
        {
            using (var sha = SHA256.Create())
                return Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(salt + password)));
        }
    }
}

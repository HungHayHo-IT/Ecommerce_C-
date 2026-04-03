using System;
using System.Security.Cryptography;

namespace SV22T1020149.Models.Security
{
    public static class PasswordHelper
    {
        private const int SaltSize = 16; // bytes
        private const int KeySize = 32; // bytes
        private const int Iterations = 100_000;

        // Stored format: pbkdf2$iterations$saltBase64$hashBase64
        public static string HashPassword(string password)
        {
            using var rng = RandomNumberGenerator.Create();
            var salt = new byte[SaltSize];
            rng.GetBytes(salt);

            using var deriveBytes = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            var key = deriveBytes.GetBytes(KeySize);

            return $"pbkdf2${Iterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(key)}";
        }

        public static bool VerifyPassword(string password, string? storedHash)
        {
            if (string.IsNullOrEmpty(storedHash)) return false;
            try
            {
                var parts = storedHash.Split('$');
                if (parts.Length != 4) return false;
                if (!int.TryParse(parts[1], out var iterations)) return false;
                var salt = Convert.FromBase64String(parts[2]);
                var key = Convert.FromBase64String(parts[3]);

                using var deriveBytes = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
                var testKey = deriveBytes.GetBytes(key.Length);
                return CryptographicOperations.FixedTimeEquals(testKey, key);
            }
            catch
            {
                return false;
            }
        }
    }
}

using System.Security.Cryptography;
using System.Text;

namespace Application.Helpers
{
    public static class HashHelper
    {
        public static string ComputeSha256Hash(string rawData)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            return Convert.ToHexString(bytes); // .NET 5+
        }

        // تشفير Encrypt
        public static string Encrypt(string plainText/*, byte[] key, byte[] iv*/)
        {
            byte[] key = Encoding.UTF8.GetBytes("12345678901234567890123456789012"); // 32 byte
            byte[] iv = Encoding.UTF8.GetBytes("1234567890123456"); // 16 byte
            using var aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;

            using var encryptor = aes.CreateEncryptor();
            var bytes = Encoding.UTF8.GetBytes(plainText);
            var encrypted = encryptor.TransformFinalBlock(bytes, 0, bytes.Length);
            return Convert.ToBase64String(encrypted);
        }

        // فك Decrypt
        public static string Decrypt(string cipherText/*, byte[] key, byte[] iv*/)
        {
            byte[] key = Encoding.UTF8.GetBytes("12345678901234567890123456789012"); // 32 byte
            byte[] iv = Encoding.UTF8.GetBytes("1234567890123456"); // 16 byte
            using var aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;

            using var decryptor = aes.CreateDecryptor();
            var bytes = Convert.FromBase64String(cipherText);
            var decrypted = decryptor.TransformFinalBlock(bytes, 0, bytes.Length);
            return Encoding.UTF8.GetString(decrypted);
        }
    }

}

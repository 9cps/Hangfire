using System.Security.Cryptography;

namespace Hangfire.Shared.Helper
{
    public static class EncryptionHelper
    {
        public static string Decrypt(this string encryptedString, string? encryptionKeyString)
        {
            //if (string.IsNullOrEmpty(encryptionKeyString))
            //    encryptionKeyString = ConfigurationHelper.config["EncryptionKey"];

            if (string.IsNullOrEmpty(encryptionKeyString))
                throw new InvalidDataException("Encryption key cannot be null or empty.");


            byte[] encryptedBytes = Convert.FromBase64String(encryptedString);
            byte[] encryptionKey = Convert.FromBase64String(encryptionKeyString);

            using Aes aes = Aes.Create();
            aes.KeySize = 256;
            aes.Key = encryptionKey;

            byte[] iv = new byte[aes.BlockSize / 8];
            Buffer.BlockCopy(encryptedBytes, 0, iv, 0, iv.Length);

            aes.IV = iv;

            using ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using MemoryStream msDecrypt = new MemoryStream(encryptedBytes, iv.Length, encryptedBytes.Length - iv.Length);
            using CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using StreamReader srDecrypt = new StreamReader(csDecrypt);
            return srDecrypt.ReadToEnd();
        }
    }
}

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public static class EncryptionHelper
{
    private static readonly string encryptionKey = "Gamebase"; // Make sure this is a secure key

    // Method to derive a key from the provided encryption key to match the required length
    private static byte[] GetKey()
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            return sha256.ComputeHash(Encoding.UTF8.GetBytes(encryptionKey)); // Use 32-byte key (256-bit)
        }
    }

    public static string Encrypt(string data)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = GetKey(); // Use derived key
            aes.IV = new byte[16]; // Initialization vector (can also be randomized and saved separately)

            using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
            using (var ms = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                using (var sw = new StreamWriter(cryptoStream))
                {
                    sw.Write(data);
                }
                return Convert.ToBase64String(ms.ToArray());
            }
        }
    }

    public static string Decrypt(string encryptedData)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = GetKey(); // Use derived key
            aes.IV = new byte[16]; // Initialization vector must match the one used in encryption

            using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
            using (var ms = new MemoryStream(Convert.FromBase64String(encryptedData)))
            using (var cryptoStream = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
            using (var sr = new StreamReader(cryptoStream))
            {
                return sr.ReadToEnd();
            }
        }
    }
}
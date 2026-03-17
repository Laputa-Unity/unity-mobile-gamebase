using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public static class EncryptionHelper
{
    private static readonly string encryptionKey = "Gamebase";

    // Format version prefix used to distinguish the new (random-IV) format from
    // the legacy format (fixed zero IV, no prefix).
    private const string NewFormatPrefix = "v2:";

    // Derives a 256-bit AES key from the encryption key using SHA-256
    private static byte[] GetKey()
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            return sha256.ComputeHash(Encoding.UTF8.GetBytes(encryptionKey));
        }
    }

    // Encrypts data using AES-256 with a random IV.
    // Output format: "v2:" + Base64([16-byte random IV][ciphertext])
    public static string Encrypt(string data)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = GetKey();
            aes.GenerateIV(); // Random IV for each encryption

            using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
            using (var ms = new MemoryStream())
            {
                ms.Write(aes.IV, 0, aes.IV.Length); // Prepend IV to ciphertext
                using (var cryptoStream = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                using (var sw = new StreamWriter(cryptoStream))
                {
                    sw.Write(data);
                }
                return NewFormatPrefix + Convert.ToBase64String(ms.ToArray());
            }
        }
    }

    // Decrypts data encrypted by Encrypt().
    // Supports both the new format ("v2:" prefix, random IV) and the legacy format (fixed zero IV).
    public static string Decrypt(string encryptedData)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = GetKey();

            if (encryptedData.StartsWith(NewFormatPrefix))
            {
                // New format: "v2:" + Base64([16-byte IV][ciphertext])
                byte[] allBytes = Convert.FromBase64String(encryptedData.Substring(NewFormatPrefix.Length));
                byte[] iv = new byte[16];
                Array.Copy(allBytes, 0, iv, 0, iv.Length);
                aes.IV = iv;

                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                using (var ms = new MemoryStream(allBytes, 16, allBytes.Length - 16))
                using (var cryptoStream = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (var sr = new StreamReader(cryptoStream))
                {
                    return sr.ReadToEnd();
                }
            }
            else
            {
                // Legacy format: Base64(ciphertext encrypted with fixed zero IV)
                aes.IV = new byte[16];
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
}
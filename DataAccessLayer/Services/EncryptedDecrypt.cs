using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace DataAccessLayer.Services
{
    public class EncryptedDecrypt
    {
        private readonly string Key;
        
        public EncryptedDecrypt(IConfiguration configuration)
        {
            Key = configuration["EncryptionSettings:AESKey"]??string.Empty; // Load Key from appsettings.json

            if (string.IsNullOrEmpty(Key) || Key.Length != 32)
            {
                throw new ArgumentException("Invalid AES Key. It must be 32 characters long.");
            }
        }
        public string Encrypt(string? plainText)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(Key);
                
                aesAlg.GenerateIV(); // Generate a new IV for each encryption
                byte[] iv = aesAlg.IV;
                aesAlg.Padding = PaddingMode.PKCS7;

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    msEncrypt.Write(iv, 0, iv.Length); // Prepend IV to the encrypted data

                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, aesAlg.CreateEncryptor(), CryptoStreamMode.Write))
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }

                    return Convert.ToBase64String(msEncrypt.ToArray()); // Convert encrypted data to Base64 string
                }
            }
        }
        public string Decrypt(string? cipherText)
        {
            byte[] fullCipher = Convert.FromBase64String(cipherText??string.Empty);

            using (Aes aesAlg = Aes.Create())
            {
                // Ensure key is 32 bytes (AES-256)
                byte[] keyBytes = Encoding.UTF8.GetBytes(Key); // Ensure 32-byte key

                if (keyBytes.Length != 16 && keyBytes.Length != 24 && keyBytes.Length != 32)
                {
                    throw new CryptographicException("Invalid AES key size. It must be 16, 24, or 32 bytes.");
                }

                aesAlg.Key = keyBytes;
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;
                //aesAlg.Padding = PaddingMode.Zeros;
                // Extract IV from the beginning of the encrypted data
                byte[] iv = new byte[aesAlg.BlockSize / 8];
                Array.Copy(fullCipher, 0, iv, 0, iv.Length);
                aesAlg.IV = iv;

                // Extract encrypted data after IV
                byte[] cipherBytes = new byte[fullCipher.Length - iv.Length];
                Array.Copy(fullCipher, iv.Length, cipherBytes, 0, cipherBytes.Length);

                using (MemoryStream msDecrypt = new MemoryStream(cipherBytes))
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, aesAlg.CreateDecryptor(), CryptoStreamMode.Read))
                using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                {
                    return srDecrypt.ReadToEnd().Trim();
                }
            }
        }

        
    }
}

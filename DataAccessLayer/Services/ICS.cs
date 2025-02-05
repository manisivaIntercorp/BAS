using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace DataAccessLayer.Services
{
    public class ICS
    {
        private static ILogger<ICS> _logger;
        private readonly byte[] _key;
        private readonly byte[] _iv;

        public ICS(ILogger<ICS> logger)
        {
            _logger = logger;
        }
        public  string Encrypt(string? Input)
        {
            return Encrypt256(Input);
        }

        public  string Decrypt(string? Input)
        {
            return Decrypt256(Input);
        }

        private static string Encrypt256(string? Input)
        {
            try {
                if (string.IsNullOrEmpty(Input ?? string.Empty))
                    throw new ArgumentException("Input cannot be null or empty.");

                string s = "!QAZ2WSX#EDC4RFV";
                string s2 = "5TGB&YHN7UJM(IK<5TGB&YHN7UJM(IK<";
                AesCryptoServiceProvider aesCryptoServiceProvider = new AesCryptoServiceProvider();
                aesCryptoServiceProvider.BlockSize = 128;
                aesCryptoServiceProvider.KeySize = 256;
                aesCryptoServiceProvider.IV = Encoding.UTF8.GetBytes(s);
                aesCryptoServiceProvider.Key = Encoding.UTF8.GetBytes(s2);
                aesCryptoServiceProvider.Mode = CipherMode.CBC;
                aesCryptoServiceProvider.Padding = PaddingMode.PKCS7;
                byte[] bytes = Encoding.Unicode.GetBytes(Input ?? string.Empty);
                using ICryptoTransform cryptoTransform = aesCryptoServiceProvider.CreateEncryptor();
                byte[] inArray = cryptoTransform.TransformFinalBlock(bytes, 0, bytes.Length);
                return Convert.ToBase64String(inArray);
            }
            catch (EncoderFallbackException ex)
            {
                _logger.LogError(ex, "EncoderFallbackException: {Message}", ex.Message);
                return null;
            }
            catch (CryptographicException ex)
            {
                _logger.LogError(ex, "CryptographicException: {Message}", ex.Message);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception: {Message}", ex.Message);
                return null;
            }
        }
            private static string Decrypt256(string? Input)
        {
            if (string.IsNullOrEmpty(Input ?? string.Empty))
                throw new ArgumentException("Input cannot be null or empty.");
            try
            {
                byte[] cipherBytes = Convert.FromBase64String(Input??string.Empty);

                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.GenerateKey();
                    aesAlg.GenerateIV();

                    aesAlg.Key = aesAlg.Key;
   
                    aesAlg.IV = aesAlg.IV;
                    aesAlg.Mode = CipherMode.CBC;
                    aesAlg.Padding = PaddingMode.PKCS7;

                    using (ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV))
                    using (MemoryStream msDecrypt = new MemoryStream(cipherBytes))
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        return srDecrypt.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Decryption failed.", ex);
            }
        }
    }
}

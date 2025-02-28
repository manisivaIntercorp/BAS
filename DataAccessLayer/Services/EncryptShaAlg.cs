﻿using System.Security.Cryptography;
using System.Text;

using Microsoft.Extensions.Logging;

namespace DataAccessLayer.Services
{
    public class EncryptShaAlg
    {
        //To Remove Warning in Logger
        private static ILogger<EncryptShaAlg> _logger = LoggerFactory
                                                                 .Create(builder => builder.AddConsole())
                                                                 .CreateLogger<EncryptShaAlg>();
        public EncryptShaAlg(ILogger<EncryptShaAlg> logger)
        {
            _logger = logger;
        }

        public static string? Encrypt(string? plainText)
        {
            try
            {
                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(plainText??string.Empty));

                    StringBuilder builder = new StringBuilder();
                    foreach (byte b in bytes)
                    {
                        builder.Append(b.ToString("x2"));
                    }
                    return builder.ToString();
                }
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex, "ArgumentNullException: {Message}", ex.Message);
                return null;
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
    }
}
   


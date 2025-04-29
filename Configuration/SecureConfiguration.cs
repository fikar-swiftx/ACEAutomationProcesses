using System;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace ACEAutomationProcesses.Configuration
{
    public static class SecureConfiguration
    {
        private static readonly string EncryptionKey = Environment.GetEnvironmentVariable("ACE_ENCRYPTION_KEY") ?? "DefaultKey123!@#";

        public static string GetConnectionString()
        {
            var encryptedConnectionString = ConfigurationManager.ConnectionStrings["DbConn"]?.ConnectionString;
            return DecryptString(encryptedConnectionString);
        }

        public static string GetOtcsUrl()
        {
            return ConfigurationManager.AppSettings["otcsURL"];
        }

        public static (string Username, string Password) GetOtcsCredentials()
        {
            var username = ConfigurationManager.AppSettings["otcsUser"];
            var encryptedPassword = ConfigurationManager.AppSettings["otcsPassword"];
            return (username, DecryptString(encryptedPassword));
        }

        private static string DecryptString(string encryptedText)
        {
            if (string.IsNullOrEmpty(encryptedText)) return string.Empty;

            try
            {
                using (var aes = Aes.Create())
                {
                    aes.Key = Encoding.UTF8.GetBytes(EncryptionKey.PadRight(32));
                    aes.IV = new byte[16];

                    var encryptedBytes = Convert.FromBase64String(encryptedText);
                    using (var decryptor = aes.CreateDecryptor())
                    {
                        var decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
                        return Encoding.UTF8.GetString(decryptedBytes);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error but don't expose sensitive information
                System.Diagnostics.Debug.WriteLine($"Error decrypting string: {ex.Message}");
                return string.Empty;
            }
        }

        public static string EncryptString(string plainText)
        {
            if (string.IsNullOrEmpty(plainText)) return string.Empty;

            try
            {
                using (var aes = Aes.Create())
                {
                    aes.Key = Encoding.UTF8.GetBytes(EncryptionKey.PadRight(32));
                    aes.IV = new byte[16];

                    using (var encryptor = aes.CreateEncryptor())
                    {
                        var plainBytes = Encoding.UTF8.GetBytes(plainText);
                        var encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
                        return Convert.ToBase64String(encryptedBytes);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error encrypting string: {ex.Message}");
                return string.Empty;
            }
        }
    }
} 
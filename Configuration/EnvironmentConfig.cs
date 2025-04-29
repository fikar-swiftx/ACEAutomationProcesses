using System;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;
using log4net;

namespace ACEAutomationProcesses.Configuration
{
    public static class EnvironmentConfig
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(EnvironmentConfig));
        private static readonly string Environment = GetEnvironment();

        public static string GetConnectionString()
        {
            var connectionString = Environment.GetEnvironmentVariable("ACE_DB_CONNECTION_STRING");
            if (string.IsNullOrEmpty(connectionString))
            {
                Log.Error("Database connection string not found in environment variables");
                throw new ConfigurationErrorsException("Database connection string not configured");
            }
            return connectionString;
        }

        public static (string Url, string Username, string Password) GetOtcsCredentials()
        {
            var url = Environment.GetEnvironmentVariable("ACE_OTCS_URL");
            var username = Environment.GetEnvironmentVariable("ACE_OTCS_USERNAME");
            var password = Environment.GetEnvironmentVariable("ACE_OTCS_PASSWORD");

            if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                Log.Error("OTCS credentials not found in environment variables");
                throw new ConfigurationErrorsException("OTCS credentials not configured");
            }

            return (url, username, password);
        }

        public static (string Server, int Port, string Username, string Password) GetSmtpSettings()
        {
            var server = Environment.GetEnvironmentVariable("ACE_SMTP_SERVER");
            var portStr = Environment.GetEnvironmentVariable("ACE_SMTP_PORT");
            var username = Environment.GetEnvironmentVariable("ACE_SMTP_USERNAME");
            var password = Environment.GetEnvironmentVariable("ACE_SMTP_PASSWORD");

            if (string.IsNullOrEmpty(server) || string.IsNullOrEmpty(portStr) || 
                string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                Log.Error("SMTP settings not found in environment variables");
                throw new ConfigurationErrorsException("SMTP settings not configured");
            }

            if (!int.TryParse(portStr, out int port))
            {
                Log.Error("Invalid SMTP port configuration");
                throw new ConfigurationErrorsException("Invalid SMTP port configuration");
            }

            return (server, port, username, password);
        }

        public static string GetNotificationEmail()
        {
            var email = Environment.GetEnvironmentVariable("ACE_NOTIFICATION_EMAIL");
            if (string.IsNullOrEmpty(email))
            {
                Log.Error("Notification email not found in environment variables");
                throw new ConfigurationErrorsException("Notification email not configured");
            }
            return email;
        }

        public static bool ValidateServerCertificate(object sender, X509Certificate certificate, 
            X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            if (Environment.Equals("Development", StringComparison.OrdinalIgnoreCase))
            {
                return true; // Allow all certificates in development
            }

            if (sslPolicyErrors == System.Net.Security.SslPolicyErrors.None)
            {
                return true;
            }

            // Get the certificate thumbprint from environment variable
            var expectedThumbprint = Environment.GetEnvironmentVariable("ACE_CERTIFICATE_THUMBPRINT");
            if (string.IsNullOrEmpty(expectedThumbprint))
            {
                Log.Error("Certificate thumbprint not configured");
                return false;
            }

            // Validate the certificate thumbprint
            return certificate.GetCertHashString().Equals(expectedThumbprint, 
                StringComparison.OrdinalIgnoreCase);
        }

        private static string GetEnvironment()
        {
            var env = Environment.GetEnvironmentVariable("ACE_ENVIRONMENT");
            return string.IsNullOrEmpty(env) ? "Development" : env;
        }
    }
} 
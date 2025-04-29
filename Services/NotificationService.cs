using System;
using System.Net.Mail;
using log4net;
using ACEAutomationProcesses.Configuration;

namespace ACEAutomationProcesses.Services
{
    public class NotificationService
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(NotificationService));

        public static void SendErrorNotification(string errorMessage, string context)
        {
            try
            {
                var (server, port, username, password) = EnvironmentConfig.GetSmtpSettings();
                var notificationEmail = EnvironmentConfig.GetNotificationEmail();

                using (var client = new SmtpClient(server, port))
                {
                    client.EnableSsl = true;
                    client.Credentials = new System.Net.NetworkCredential(username, password);

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(username),
                        Subject = $"Critical Error in ACE Automation Process - {context}",
                        Body = $"A critical error occurred in the ACE Automation Process:\n\n" +
                               $"Context: {context}\n" +
                               $"Error Message: {errorMessage}\n" +
                               $"Time: {DateTime.Now}\n\n" +
                               "Please check the application logs for more details.",
                        IsBodyHtml = false
                    };

                    mailMessage.To.Add(notificationEmail);

                    client.Send(mailMessage);
                    Log.Info($"Error notification sent for context: {context}");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to send error notification: {ex.Message}");
            }
        }
    }
} 
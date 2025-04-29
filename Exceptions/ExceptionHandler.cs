using System;
using System.IO;
using log4net;
using ACEAutomationProcesses.Services;

namespace ACEAutomationProcesses.Exceptions
{
    public static class ExceptionHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ExceptionHandler));

        public static void HandleException(Exception ex, string context = null)
        {
            var errorMessage = $"Error in {context ?? "unknown context"}: {ex.Message}";
            var fullError = $"{errorMessage}\nStack Trace: {ex.StackTrace}";

            // Log the error
            Log.Error(fullError);

            // If it's a critical error, notify administrators
            if (IsCriticalError(ex))
            {
                NotificationService.SendErrorNotification(errorMessage, context);
            }

            // Write to error log
            WriteToErrorLog(fullError);
        }

        private static bool IsCriticalError(Exception ex)
        {
            return ex is OutOfMemoryException ||
                   ex is StackOverflowException ||
                   ex is ThreadAbortException ||
                   ex is System.Data.SqlClient.SqlException ||
                   ex is System.Net.WebException;
        }

        private static void WriteToErrorLog(string errorMessage)
        {
            try
            {
                var errorLogPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "ErrorLog.txt");
                Directory.CreateDirectory(Path.GetDirectoryName(errorLogPath));
                File.AppendAllText(errorLogPath, $"{DateTime.Now}: {errorMessage}\n");
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to write to error log: {ex.Message}");
            }
        }
    }
} 
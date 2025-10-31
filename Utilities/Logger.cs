using System;
using System.IO;

namespace Utilities
{
    public static class Logger
    {
        private static readonly string logFilePath = "application.log";

        public static void LogInfo(string message)
        {
            Log("INFO", message);
        }

        public static void LogWarning(string message)
        {
            Log("WARNING", message);
        }

        public static void LogError(string message, Exception ex = null)
        {
            string errorMessage = message;
            if (ex != null)
            {
                errorMessage += $"\nException: {ex.Message}\nStack Trace: {ex.StackTrace}";
            }
            Log("ERROR", errorMessage);
        }

        private static void Log(string level, string message)
        {
            try
            {
                string logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{level}] {message}";

                using (StreamWriter writer = new StreamWriter(logFilePath, true))
                {
                    writer.WriteLine(logMessage);
                }

                // Also output to console for debugging
                Console.WriteLine(logMessage);
            }
            catch
            {
                // Ignore logging errors
            }
        }
    }
}
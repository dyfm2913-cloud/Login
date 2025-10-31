using System;
using System.IO;

namespace Utilities
{
    public static class Logger
    {
        private static readonly string LogPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");

        static Logger()
        {
            if (!Directory.Exists(LogPath))
            {
                Directory.CreateDirectory(LogPath);
            }
        }

        public static void LogError(string message, Exception ex = null)
        {
            try
            {
                string logFile = Path.Combine(LogPath, $"Error_{DateTime.Now:yyyyMMdd}.log");
                string logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - ERROR: {message}";

                if (ex != null)
                {
                    logMessage += $"\nException: {ex.Message}\nStackTrace: {ex.StackTrace}";
                }

                logMessage += "\n" + new string('-', 50) + "\n";

                File.AppendAllText(logFile, logMessage);
            }
            catch
            {
                // لا نريد أن يتسبب التسجيل في حدوث أخطاء
            }
        }

        public static void LogInfo(string message)
        {
            try
            {
                string logFile = Path.Combine(LogPath, $"Info_{DateTime.Now:yyyyMMdd}.log");
                string logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - INFO: {message}\n";

                File.AppendAllText(logFile, logMessage);
            }
            catch
            {
                // لا نريد أن يتسبب التسجيل في حدوث أخطاء
            }
        }
    }
}
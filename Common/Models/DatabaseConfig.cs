using System;

namespace Common.Models
{
    public class DatabaseConfig
    {
        public string ServerName { get; set; }
        public string DatabaseName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool UseWindowsAuthentication { get; set; }
        public int Timeout { get; set; }

        public DatabaseConfig()
        {
            ServerName = ".";
            DatabaseName = "MasterDB";
            UseWindowsAuthentication = true;
            Timeout = 30;
            Username = string.Empty;
            Password = string.Empty;
        }

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(ServerName) &&
                   !string.IsNullOrWhiteSpace(DatabaseName);
        }

        public string GetConnectionString()
        {
            if (UseWindowsAuthentication)
            {
                return $"Server={ServerName};Database={DatabaseName};Integrated Security=true;Timeout={Timeout}";
            }
            else
            {
                return $"Server={ServerName};Database={DatabaseName};User Id={Username};Password={Password};Timeout={Timeout}";
            }
        }
    }
}
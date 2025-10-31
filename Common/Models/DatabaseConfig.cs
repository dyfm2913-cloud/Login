using System;

namespace Common.Models
{
    [Serializable]
    public class DatabaseConfig
    {
        public string ServerName { get; set; }
        public string DatabaseName { get; set; }
        public bool UseWindowsAuthentication { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public DatabaseConfig()
        {
            ServerName = ".";
            DatabaseName = "LoginDB";
            UseWindowsAuthentication = true;
        }

        public DatabaseConfig(string serverName, string databaseName, bool useWindowsAuth = true, string username = "", string password = "")
        {
            ServerName = serverName;
            DatabaseName = databaseName;
            UseWindowsAuthentication = useWindowsAuth;
            Username = username;
            Password = password;
        }

        public string GetConnectionString()
        {
            if (UseWindowsAuthentication)
            {
                return $"Server={ServerName};Database={DatabaseName};Integrated Security=true;";
            }
            else
            {
                return $"Server={ServerName};Database={DatabaseName};User Id={Username};Password={Password};";
            }
        }

        public override string ToString()
        {
            return UseWindowsAuthentication
                ? $"Server: {ServerName}, Database: {DatabaseName}, Windows Auth"
                : $"Server: {ServerName}, Database: {DatabaseName}, User: {Username}";
        }
    }
}
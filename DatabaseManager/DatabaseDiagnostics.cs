using System;
using System.Data;
using System.Data.SqlClient;

namespace DatabaseManager
{
    public class DatabaseManager : IDisposable
    {
        private SqlConnection connection;
        private string connectionString;

        public DatabaseManager(string connectionString)
        {
            this.connectionString = connectionString;
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            try
            {
                // Create database and table if they don't exist
                string createDbQuery = @"
                    IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = 'LoginDB')
                    BEGIN
                        CREATE DATABASE LoginDB;
                    END";

                string createTableQuery = @"
                    USE LoginDB;
                    IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Users' and xtype='U')
                    BEGIN
                        CREATE TABLE Users (
                            Id INT IDENTITY(1,1) PRIMARY KEY,
                            Username NVARCHAR(50) UNIQUE NOT NULL,
                            PasswordHash NVARCHAR(255) NOT NULL,
                            Email NVARCHAR(100),
                            IsActive BIT DEFAULT 1,
                            CreatedDate DATETIME DEFAULT GETDATE()
                        );

                        -- Insert sample user
                        INSERT INTO Users (Username, PasswordHash, Email) 
                        VALUES ('admin', 'hashed_password_here', 'admin@example.com');
                    END";

                using (var masterConnection = new SqlConnection("Server=.;Database=master;Integrated Security=true;"))
                {
                    masterConnection.Open();
                    using (var cmd = new SqlCommand(createDbQuery, masterConnection))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }

                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (var cmd = new SqlCommand(createTableQuery, connection))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Database initialization failed: {ex.Message}");
            }
        }

        public SqlConnection GetConnection()
        {
            if (connection == null || connection.State != ConnectionState.Open)
            {
                connection = new SqlConnection(connectionString);
                connection.Open();
            }
            return connection;
        }

        public DataTable ExecuteQuery(string query, SqlParameter[] parameters = null)
        {
            try
            {
                using (var connection = GetConnection())
                using (var command = new SqlCommand(query, connection))
                {
                    if (parameters != null)
                        command.Parameters.AddRange(parameters);

                    var dataTable = new DataTable();
                    using (var adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(dataTable);
                    }
                    return dataTable;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Query execution failed: {ex.Message}", ex);
            }
        }

        public int ExecuteNonQuery(string query, SqlParameter[] parameters = null)
        {
            try
            {
                using (var connection = GetConnection())
                using (var command = new SqlCommand(query, connection))
                {
                    if (parameters != null)
                        command.Parameters.AddRange(parameters);

                    return command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Non-query execution failed: {ex.Message}", ex);
            }
        }

        public T ExecuteScalar<T>(string query, SqlParameter[] parameters = null)
        {
            try
            {
                using (var connection = GetConnection())
                using (var command = new SqlCommand(query, connection))
                {
                    if (parameters != null)
                        command.Parameters.AddRange(parameters);

                    var result = command.ExecuteScalar();
                    return result != null ? (T)Convert.ChangeType(result, typeof(T)) : default(T);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Scalar execution failed: {ex.Message}", ex);
            }
        }

        public void Dispose()
        {
            connection?.Close();
            connection?.Dispose();
        }
    }
}
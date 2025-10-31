using System;
using System.Data;
using System.Data.SqlClient;
using Common.Models;

namespace DatabaseManager
{
    public class DatabaseService : IDisposable
    {
        private SqlConnection _connection;
        private string _connectionString;

        public DatabaseService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DatabaseService(DatabaseConfig config)
        {
            _connectionString = config.GetConnectionString();
        }

        public SqlConnection GetConnection()
        {
            if (_connection == null || _connection.State != ConnectionState.Open)
            {
                _connection = new SqlConnection(_connectionString);
                _connection.Open();
            }
            return _connection;
        }

        public DataTable ExecuteQuery(string query, SqlParameter[] parameters = null)
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

        public int ExecuteNonQuery(string query, SqlParameter[] parameters = null)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand(query, connection))
            {
                if (parameters != null)
                    command.Parameters.AddRange(parameters);

                return command.ExecuteNonQuery();
            }
        }

        public T ExecuteScalar<T>(string query, SqlParameter[] parameters = null)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand(query, connection))
            {
                if (parameters != null)
                    command.Parameters.AddRange(parameters);

                var result = command.ExecuteScalar();
                return result != DBNull.Value ? (T)Convert.ChangeType(result, typeof(T)) : default(T);
            }
        }

        public void Dispose()
        {
            _connection?.Close();
            _connection?.Dispose();
        }
    }
}
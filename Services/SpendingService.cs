using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Common.Models;
using DatabaseManager;
using Utilities;

namespace Services
{
    public interface ISpendingService
    {
        List<SpendingView> GetAllSpendings();
        SpendingView GetSpendingById(int id);
        List<SpendingView> GetSpendingsByUserId(int userId);
        bool AddSpending(Spending spending);
        bool UpdateSpending(Spending spending);
        bool DeleteSpending(int id);
        List<SpendingView> SearchSpendings(string searchTerm);
        decimal GetTotalSpent(int userId);
    }

    public class SpendingService : ISpendingService
    {
        private readonly DatabaseService _databaseService;

        public SpendingService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public SpendingService(string connectionString)
        {
            _databaseService = new DatabaseService(connectionString);
        }

        public List<SpendingView> GetAllSpendings()
        {
            var spendings = new List<SpendingView>();

            try
            {
                string query = @"
                    SELECT s.*, u.Username 
                    FROM Spendings s 
                    INNER JOIN Users u ON s.UserId = u.Id 
                    ORDER BY s.SpendingDate DESC";

                var dataTable = _databaseService.ExecuteQuery(query);

                foreach (DataRow row in dataTable.Rows)
                {
                    spendings.Add(new SpendingView
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        UserId = Convert.ToInt32(row["UserId"]),
                        Username = row["Username"].ToString(),
                        Amount = Convert.ToDecimal(row["Amount"]),
                        Description = row["Description"].ToString(),
                        Category = row["Category"].ToString(),
                        SpendingDate = Convert.ToDateTime(row["SpendingDate"]),
                        CreatedAt = Convert.ToDateTime(row["CreatedAt"])
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Error getting all spendings", ex);
                throw;
            }

            return spendings;
        }

        public SpendingView GetSpendingById(int id)
        {
            try
            {
                string query = @"
                    SELECT s.*, u.Username 
                    FROM Spendings s 
                    INNER JOIN Users u ON s.UserId = u.Id 
                    WHERE s.Id = @Id";

                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@Id", id)
                };

                var dataTable = _databaseService.ExecuteQuery(query, parameters);

                if (dataTable.Rows.Count > 0)
                {
                    DataRow row = dataTable.Rows[0];
                    return new SpendingView
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        UserId = Convert.ToInt32(row["UserId"]),
                        Username = row["Username"].ToString(),
                        Amount = Convert.ToDecimal(row["Amount"]),
                        Description = row["Description"].ToString(),
                        Category = row["Category"].ToString(),
                        SpendingDate = Convert.ToDateTime(row["SpendingDate"]),
                        CreatedAt = Convert.ToDateTime(row["CreatedAt"])
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error getting spending by ID: {id}", ex);
                throw;
            }

            return null;
        }

        public List<SpendingView> GetSpendingsByUserId(int userId)
        {
            var spendings = new List<SpendingView>();

            try
            {
                string query = @"
                    SELECT s.*, u.Username 
                    FROM Spendings s 
                    INNER JOIN Users u ON s.UserId = u.Id 
                    WHERE s.UserId = @UserId 
                    ORDER BY s.SpendingDate DESC";

                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@UserId", userId)
                };

                var dataTable = _databaseService.ExecuteQuery(query, parameters);

                foreach (DataRow row in dataTable.Rows)
                {
                    spendings.Add(new SpendingView
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        UserId = Convert.ToInt32(row["UserId"]),
                        Username = row["Username"].ToString(),
                        Amount = Convert.ToDecimal(row["Amount"]),
                        Description = row["Description"].ToString(),
                        Category = row["Category"].ToString(),
                        SpendingDate = Convert.ToDateTime(row["SpendingDate"]),
                        CreatedAt = Convert.ToDateTime(row["CreatedAt"])
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error getting spendings for user: {userId}", ex);
                throw;
            }

            return spendings;
        }

        public bool AddSpending(Spending spending)
        {
            try
            {
                string query = @"
                    INSERT INTO Spendings (UserId, Amount, Description, Category, SpendingDate, CreatedAt)
                    VALUES (@UserId, @Amount, @Description, @Category, @SpendingDate, GETDATE())";

                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@UserId", spending.UserId),
                    new SqlParameter("@Amount", spending.Amount),
                    new SqlParameter("@Description", spending.Description),
                    new SqlParameter("@Category", spending.Category),
                    new SqlParameter("@SpendingDate", spending.SpendingDate)
                };

                int rowsAffected = _databaseService.ExecuteNonQuery(query, parameters);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Logger.LogError("Error adding spending", ex);
                return false;
            }
        }

        public bool UpdateSpending(Spending spending)
        {
            try
            {
                string query = @"
                    UPDATE Spendings 
                    SET Amount = @Amount, 
                        Description = @Description, 
                        Category = @Category, 
                        SpendingDate = @SpendingDate
                    WHERE Id = @Id";

                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@Id", spending.Id),
                    new SqlParameter("@Amount", spending.Amount),
                    new SqlParameter("@Description", spending.Description),
                    new SqlParameter("@Category", spending.Category),
                    new SqlParameter("@SpendingDate", spending.SpendingDate)
                };

                int rowsAffected = _databaseService.ExecuteNonQuery(query, parameters);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error updating spending: {spending.Id}", ex);
                return false;
            }
        }

        public bool DeleteSpending(int id)
        {
            try
            {
                string query = "DELETE FROM Spendings WHERE Id = @Id";
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@Id", id)
                };

                int rowsAffected = _databaseService.ExecuteNonQuery(query, parameters);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error deleting spending: {id}", ex);
                return false;
            }
        }

        public List<SpendingView> SearchSpendings(string searchTerm)
        {
            var spendings = new List<SpendingView>();

            try
            {
                string query = @"
                    SELECT s.*, u.Username 
                    FROM Spendings s 
                    INNER JOIN Users u ON s.UserId = u.Id 
                    WHERE s.Description LIKE @SearchTerm 
                       OR s.Category LIKE @SearchTerm 
                       OR u.Username LIKE @SearchTerm
                    ORDER BY s.SpendingDate DESC";

                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@SearchTerm", $"%{searchTerm}%")
                };

                var dataTable = _databaseService.ExecuteQuery(query, parameters);

                foreach (DataRow row in dataTable.Rows)
                {
                    spendings.Add(new SpendingView
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        UserId = Convert.ToInt32(row["UserId"]),
                        Username = row["Username"].ToString(),
                        Amount = Convert.ToDecimal(row["Amount"]),
                        Description = row["Description"].ToString(),
                        Category = row["Category"].ToString(),
                        SpendingDate = Convert.ToDateTime(row["SpendingDate"]),
                        CreatedAt = Convert.ToDateTime(row["CreatedAt"])
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error searching spendings: {searchTerm}", ex);
                throw;
            }

            return spendings;
        }

        public decimal GetTotalSpent(int userId)
        {
            try
            {
                string query = "SELECT SUM(Amount) FROM Spendings WHERE UserId = @UserId";
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@UserId", userId)
                };

                var result = _databaseService.ExecuteScalar<object>(query, parameters);
                return result == DBNull.Value ? 0 : Convert.ToDecimal(result);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error getting total spent for user: {userId}", ex);
                return 0;
            }
        }
    }

    public class Spending
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public DateTime SpendingDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class SpendingView
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public DateTime SpendingDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
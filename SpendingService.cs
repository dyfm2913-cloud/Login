using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Common.Models;
using Common.Interfaces;
using DatabaseManager;
using Common.Enums;

namespace Services
{
    public class SpendingService : ISpendingService
    {
        private readonly DatabaseService _databaseService;

        public SpendingService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public List<SpendingView> GetAllSpending()
        {
            var spendingList = new List<SpendingView>();

            try
            {
                using (var connection = _databaseService.GetConnection(DatabaseType.AppDatabase))
                {
                    connection.Open();
                    using (var command = new SqlCommand("SELECT * FROM vewSpending ORDER BY التاريخ DESC, الرقم DESC", connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var spending = MapReaderToSpendingView(reader);
                                spendingList.Add(spending);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"خطأ في جلب جميع سندات الصرف: {ex.Message}", ex);
            }

            return spendingList;
        }

        public List<SpendingView> GetSpendingByDate(DateTime date)
        {
            var spendingList = new List<SpendingView>();

            try
            {
                using (var connection = _databaseService.GetConnection(DatabaseType.AppDatabase))
                {
                    connection.Open();
                    using (var command = new SqlCommand(
                        "SELECT * FROM vewSpending WHERE CONVERT(DATE, التاريخ) = @Date ORDER BY الرقم DESC",
                        connection))
                    {
                        command.Parameters.AddWithValue("@Date", date.Date);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var spending = MapReaderToSpendingView(reader);
                                spendingList.Add(spending);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"خطأ في جلب سندات الصرف للتاريخ {date:yyyy/MM/dd}: {ex.Message}", ex);
            }

            return spendingList;
        }

        public List<SpendingView> GetSpendingByDateRange(DateTime startDate, DateTime endDate)
        {
            var spendingList = new List<SpendingView>();

            try
            {
                using (var connection = _databaseService.GetConnection(DatabaseType.AppDatabase))
                {
                    connection.Open();
                    using (var command = new SqlCommand(
                        "SELECT * FROM vewSpending WHERE CONVERT(DATE, التاريخ) BETWEEN @StartDate AND @EndDate ORDER BY التاريخ DESC, الرقم DESC",
                        connection))
                    {
                        command.Parameters.AddWithValue("@StartDate", startDate.Date);
                        command.Parameters.AddWithValue("@EndDate", endDate.Date);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var spending = MapReaderToSpendingView(reader);
                                spendingList.Add(spending);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"خطأ في جلب سندات الصرف للفترة {startDate:yyyy/MM/dd} - {endDate:yyyy/MM/dd}: {ex.Message}", ex);
            }

            return spendingList;
        }

        public SpendingView GetSpendingById(long id)
        {
            try
            {
                using (var connection = _databaseService.GetConnection(DatabaseType.AppDatabase))
                {
                    connection.Open();
                    using (var command = new SqlCommand("SELECT * FROM vewSpending WHERE ID = @ID", connection))
                    {
                        command.Parameters.AddWithValue("@ID", id);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return MapReaderToSpendingView(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"خطأ في جلب سند الصرف بالمعرف {id}: {ex.Message}", ex);
            }

            return null;
        }

        public bool AddSpending(Spending spending)
        {
            try
            {
                using (var connection = _databaseService.GetConnection(DatabaseType.AppDatabase))
                {
                    connection.Open();

                    using (var command = new SqlCommand(@"
                        INSERT INTO tblSpending 
                        (TheNumber, TheDate, TheMethod, Amount, CurrencyID, AccountID, Notes, UserID, EnterTime) 
                        VALUES 
                        (@TheNumber, @TheDate, @TheMethod, @Amount, @CurrencyID, @AccountID, @Notes, @UserID, @EnterTime)",
                        connection))
                    {
                        command.Parameters.AddWithValue("@TheNumber", spending.TheNumber);
                        command.Parameters.AddWithValue("@TheDate", spending.TheDate);
                        command.Parameters.AddWithValue("@TheMethod", spending.TheMethod);
                        command.Parameters.AddWithValue("@Amount", spending.Amount);
                        command.Parameters.AddWithValue("@CurrencyID", spending.CurrencyID);
                        command.Parameters.AddWithValue("@AccountID", spending.AccountID);
                        command.Parameters.AddWithValue("@Notes", spending.Notes ?? string.Empty);
                        command.Parameters.AddWithValue("@UserID", spending.UserID);
                        command.Parameters.AddWithValue("@EnterTime", spending.EnterTime);

                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"خطأ في إضافة سند الصرف: {ex.Message}", ex);
            }
        }

        public bool UpdateSpending(Spending spending)
        {
            try
            {
                using (var connection = _databaseService.GetConnection(DatabaseType.AppDatabase))
                {
                    connection.Open();

                    using (var command = new SqlCommand(@"
                        UPDATE tblSpending 
                        SET TheNumber = @TheNumber, TheDate = @TheDate, TheMethod = @TheMethod, 
                            Amount = @Amount, CurrencyID = @CurrencyID, AccountID = @AccountID, 
                            Notes = @Notes, UserID = @UserID
                        WHERE ID = @ID",
                        connection))
                    {
                        command.Parameters.AddWithValue("@ID", spending.ID);
                        command.Parameters.AddWithValue("@TheNumber", spending.TheNumber);
                        command.Parameters.AddWithValue("@TheDate", spending.TheDate);
                        command.Parameters.AddWithValue("@TheMethod", spending.TheMethod);
                        command.Parameters.AddWithValue("@Amount", spending.Amount);
                        command.Parameters.AddWithValue("@CurrencyID", spending.CurrencyID);
                        command.Parameters.AddWithValue("@AccountID", spending.AccountID);
                        command.Parameters.AddWithValue("@Notes", spending.Notes ?? string.Empty);
                        command.Parameters.AddWithValue("@UserID", spending.UserID);

                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"خطأ في تحديث سند الصرف {spending.ID}: {ex.Message}", ex);
            }
        }

        public bool DeleteSpending(long spendingID)
        {
            try
            {
                using (var connection = _databaseService.GetConnection(DatabaseType.AppDatabase))
                {
                    connection.Open();

                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            using (var command = new SqlCommand("DELETE FROM tblSpending WHERE ID = @ID", connection, transaction))
                            {
                                command.Parameters.AddWithValue("@ID", spendingID);
                                int rowsAffected = command.ExecuteNonQuery();

                                transaction.Commit();
                                return rowsAffected > 0;
                            }
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"خطأ في حذف سند الصرف: {ex.Message}", ex);
            }
        }

        public long GetNextVoucherNumber()
        {
            try
            {
                using (var connection = _databaseService.GetConnection(DatabaseType.AppDatabase))
                {
                    connection.Open();
                    using (var command = new SqlCommand("SELECT ISNULL(MAX(TheNumber), 0) + 1 FROM tblSpending", connection))
                    {
                        var result = command.ExecuteScalar();
                        return result != DBNull.Value ? Convert.ToInt64(result) : 1;
                    }
                }
            }
            catch (Exception ex)
            {
                return 1;
            }
        }

        public bool ValidateSpending(Spending spending)
        {
            if (spending == null)
                return false;

            if (spending.Amount <= 0)
                return false;

            if (spending.TheDate > DateTime.Now)
                return false;

            if (string.IsNullOrWhiteSpace(spending.Notes) && spending.Amount > 1000)
                return false;

            return true;
        }

        private SpendingView MapReaderToSpendingView(SqlDataReader reader)
        {
            return new SpendingView
            {
                ID = reader["ID"] != DBNull.Value ? Convert.ToInt64(reader["ID"]) : 0L,
                الرقم = reader["الرقم"] != DBNull.Value ? Convert.ToInt64(reader["الرقم"]) : (long?)null,
                التاريخ = reader["التاريخ"] != DBNull.Value ? Convert.ToDateTime(reader["التاريخ"]) : (DateTime?)null,
                طريقة_الصرف = reader["طريقة الصرف"] != DBNull.Value ? reader["طريقة الصرف"].ToString() : string.Empty,
                المبلغ = reader["المبلغ"] != DBNull.Value ? Convert.ToDecimal(reader["المبلغ"]) : 0m,
                العملة = reader["العملة"] != DBNull.Value ? reader["العملة"].ToString() : string.Empty,
                الصندوق = reader["الصندوق"] != DBNull.Value ? reader["الصندوق"].ToString() : string.Empty,
                مبلغ_الحساب = reader["مبلغ الحساب"] != DBNull.Value ? Convert.ToDecimal(reader["مبلغ الحساب"]) : 0m,
                عملة_الحساب = reader["عملة الحساب"] != DBNull.Value ? reader["عملة الحساب"].ToString() : string.Empty,
                اسم_الحساب = reader["اسم الحساب"] != DBNull.Value ? reader["اسم الحساب"].ToString() : string.Empty,
                ملاحظات = reader["ملاحظات"] != DBNull.Value ? reader["ملاحظات"].ToString() : string.Empty,
                رقم_المرجع = reader["رقم المرجع"] != DBNull.Value ? reader["رقم المرجع"].ToString() : string.Empty,
                مناولة = reader["مناولة"] != DBNull.Value ? reader["مناولة"].ToString() : string.Empty,
                مركز_التكلفة = reader["مركز التكلفة"] != DBNull.Value ? reader["مركز التكلفة"].ToString() : string.Empty,
                المستخدم = reader["المستخدم"] != DBNull.Value ? reader["المستخدم"].ToString() : string.Empty,
                الفرع = reader["الفرع"] != DBNull.Value ? reader["الفرع"].ToString() : string.Empty,
                وقت_الإدخال = reader["وقت الإدخال"] != DBNull.Value ? Convert.ToDateTime(reader["وقت الإدخال"]) : (DateTime?)null,
                الطبعات = reader["الطبعات"] != DBNull.Value ? Convert.ToInt32(reader["الطبعات"]) : 0,
                رقم_الشيك_الخاص = reader["رقم الشيك الخاص"] != DBNull.Value ? reader["رقم الشيك الخاص"].ToString() : string.Empty,
                اسم_المفوض = reader["اسم المفوض"] != DBNull.Value ? reader["اسم المفوض"].ToString() : string.Empty,
                الصرف_للمفوض = reader["الصرف للمفوض"] != DBNull.Value ? Convert.ToBoolean(reader["الصرف للمفوض"]) : false,
                معتمد = reader["معتمد"] != DBNull.Value ? Convert.ToBoolean(reader["معتمد"]) : (bool?)null,
                المعتمد = reader["المعتمد"] != DBNull.Value ? reader["المعتمد"].ToString() : string.Empty,
                الفئات = reader["الفئات"] != DBNull.Value ? reader["الفئات"].ToString() : string.Empty
            };
        }
    }
}
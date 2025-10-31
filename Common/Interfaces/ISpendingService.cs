using System;
using System.Collections.Generic;
using Common.Models;

namespace Common.Interfaces
{
    public interface ISpendingService
    {
        List<SpendingView> GetAllSpending();
        List<SpendingView> GetSpendingByDate(DateTime date);
        List<SpendingView> GetSpendingByDateRange(DateTime startDate, DateTime endDate);
        SpendingView GetSpendingById(long id);
        bool AddSpending(Spending spending);
        bool UpdateSpending(Spending spending);
        bool DeleteSpending(long spendingID);
        long GetNextVoucherNumber();
        bool ValidateSpending(Spending spending);
        List<SpendingView> SearchSpending(string searchTerm, DateTime? fromDate, DateTime? toDate);
    }
}
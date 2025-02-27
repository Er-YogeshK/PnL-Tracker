using PnL.Data;
using PnL.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace PnL.Services
{

    public class ProfitLossService : IProfitLossService
    {
        private readonly AppDbContext _context;

        public ProfitLossService(AppDbContext context)
        {
            _context = context;
        }

        public void CalculateDailyProfitLossForDate(DateTime transactionDate)
        {
            var transactionsForDate = _context.Transactions
                .Where(t => t.Date.Date == transactionDate.Date)
                .ToList();

            var dailySummary = _context.DailySummaries
                .Include(d => d.Transactions)
                .FirstOrDefault(d => d.Date == transactionDate.Date);

            if (dailySummary == null)
            {
                // Create a new summary for the date
                dailySummary = new DailySummary
                {
                    Date = transactionDate.Date,
                    Transactions = new List<Transaction>(transactionsForDate)
                };
                _context.DailySummaries.Add(dailySummary);
            }
            else
            {
                // Clear and update transactions for the specific date
                dailySummary.Transactions.Clear();
                dailySummary.Transactions.AddRange(transactionsForDate);
            }

            _context.SaveChanges();
        }

    }
}

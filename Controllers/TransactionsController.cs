using Microsoft.AspNetCore.Mvc;
using PnL.Data;
using PnL.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using PnL.Services;

namespace PnL.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IProfitLossService _profitLossService;

        public TransactionsController(AppDbContext context, IProfitLossService profitLossService)
        {
            _context = context;
            _profitLossService = profitLossService;
        }

        public IActionResult Index(int? year)
        {
            var transactions = _context.Transactions.AsQueryable();

            if (year.HasValue && year > 0)
            {
                transactions = transactions.Where(t => t.Date.Year == year);
            }

            var orderedTransactions = transactions.OrderByDescending(t => t.Date).ToList();
            ViewBag.SelectedYear = year ?? -1; // Pass selected year to the view
            return View(orderedTransactions);
        }

        [HttpPost]
        public IActionResult AddTransaction([FromBody] TransactionEntryDto entry)
        {
            if (entry == null || entry.Date == null)
                return BadRequest(new { message = "Invalid data" });

            if (!DateTime.TryParse(entry.Date, out DateTime transactionDate))
                return BadRequest(new { message = "Invalid date format" });

            var transaction = new Transaction
            {
                Date = transactionDate,
                Deposit = entry.Deposit > 0 ? entry.Deposit : 0,
                Withdrawal = entry.Withdrawal > 0 ? entry.Withdrawal : 0
            };

            _context.Transactions.Add(transaction);
            _context.SaveChanges();

            // Update only for the specific date to avoid unnecessary recalculations
            _profitLossService.CalculateDailyProfitLossForDate(transactionDate);

            return Ok(new { message = "Transaction added successfully" });
        }

        [HttpPut]
        public IActionResult UpdateTransaction([FromBody] Transaction transaction)
        {
            var existingTransaction = _context.Transactions.FirstOrDefault(t => t.Id == transaction.Id);
            if (existingTransaction == null)
                return NotFound(new { message = "Transaction not found" });

            existingTransaction.Date = transaction.Date;
            existingTransaction.Deposit = transaction.Deposit;
            existingTransaction.Withdrawal = transaction.Withdrawal;

            _context.SaveChanges();

            // Update only for the specific date to avoid unnecessary recalculations
            // _profitLossService.CalculateDailyProfitLossForDate(transaction.Date);
            return Ok(new { message = "Transaction updated successfully" });
        }

        [HttpGet]
        public IActionResult DeleteTransaction([FromQuery] int id)
        {
            var transaction = _context.Transactions.FirstOrDefault(t => t.Id == id);
            if (transaction == null)
                return NotFound(new { message = "Transaction not found" });

            _context.Transactions.Remove(transaction);
            _context.SaveChanges();

            // Update only for the specific date to avoid unnecessary recalculations
            // _profitLossService.CalculateDailyProfitLossForDate(transaction.Date);

            return Ok(new { message = "Transaction deleted successfully" });
        }
    }
}

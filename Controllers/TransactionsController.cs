using Microsoft.AspNetCore.Mvc;
using AviatorPnL.Data;
using AviatorPnL.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AviatorPnL.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly AppDbContext _context;

        public TransactionsController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var transactions = _context.Transactions.OrderByDescending(t => t.Date).ToList();
            return View(transactions);
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
            return Ok(new { message = "Transaction updated successfully" });
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteTransaction(int id)
        {
            var transaction = _context.Transactions.FirstOrDefault(t => t.Id == id);
            if (transaction == null)
                return NotFound(new { message = "Transaction not found" });

            _context.Transactions.Remove(transaction);
            _context.SaveChanges();

            return Ok(new { message = "Transaction deleted successfully" });
        }
    }
}

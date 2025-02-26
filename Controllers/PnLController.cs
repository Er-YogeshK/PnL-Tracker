using Microsoft.AspNetCore.Mvc;
using AviatorPnL.Data;
using AviatorPnL.Models;
using AviatorPnL.Services;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace AviatorPnL.Controllers
{
    public class PnLController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IProfitLossService _profitLossService;

        public PnLController(AppDbContext context, IProfitLossService profitLossService)
        {
            _context = context;
            _profitLossService = profitLossService;
        }

        public IActionResult Index()
        {
            var dailySummaries = _context.DailySummaries
                .Include(d => d.Transactions) // Ensure transactions are loaded
                .ToList();

            return View(dailySummaries);
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

    }
}

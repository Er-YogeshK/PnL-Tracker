using Microsoft.AspNetCore.Mvc;
using PnL.Data;
using PnL.Models;
using PnL.Services;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace PnL.Controllers
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

        public IActionResult Index(int? year)
        {
            int selectedYear = year ?? DateTime.Now.Year; // Default to current year

            var dailySummaries = _context.DailySummaries
                .Include(d => d.Transactions)
                .Where(d => d.Date.Year == selectedYear) // Ensure filtering by year
                .ToList();

            ViewBag.SelectedYear = selectedYear; // Pass selected year to the view
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

        [HttpGet]
        public JsonResult GetYearlyProfitLoss(int? year)
        {
            int selectedYear = year ?? DateTime.Now.Year; // Default to current year

            var yearlyTransactions = _context.DailySummaries
                .Where(d => d.Date.Year == selectedYear)
                .SelectMany(d => d.Transactions)
                .ToList();

            decimal totalDeposit = yearlyTransactions.Sum(t => t.Deposit);
            decimal totalWithdrawal = yearlyTransactions.Sum(t => t.Withdrawal);
            decimal totalProfitLoss = totalWithdrawal - totalDeposit; // Net P&L

            return Json(new { profitLoss = totalProfitLoss, year = selectedYear });
        }

        [HttpGet]
        public JsonResult GetOverallProfitLoss()
        {
            var transactions = _context.DailySummaries
            .SelectMany(d => d.Transactions)
            .ToList();

            decimal totalDeposit = transactions.Sum(t => t.Deposit);
            decimal totalWithdrawal = transactions.Sum(t => t.Withdrawal);
            decimal totalProfitLoss = totalWithdrawal - totalDeposit; // Net P&L

            return Json(new { profitLoss = totalProfitLoss });
        }
    }
}

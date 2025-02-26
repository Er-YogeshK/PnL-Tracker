using AviatorPnL.Models;
using Microsoft.EntityFrameworkCore;

namespace AviatorPnL.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<DailySummary> DailySummaries { get; set; }
    }
}

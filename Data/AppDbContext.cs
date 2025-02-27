using PnL.Models;
using Microsoft.EntityFrameworkCore;

namespace PnL.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<DailySummary> DailySummaries { get; set; }

        // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        // {
        //     if (!optionsBuilder.IsConfigured)
        //     {
        //         optionsBuilder.UseSqlite("Data Source=database.db");
        //     }
        // }
    }
}

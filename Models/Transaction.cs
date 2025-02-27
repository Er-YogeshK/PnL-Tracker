using System;

namespace PnL.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal Deposit { get; set; }
        public decimal Withdrawal { get; set; }
    }
}

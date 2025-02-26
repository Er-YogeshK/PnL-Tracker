using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AviatorPnL.Models
{
    public class DailySummary
    {
        [Key]
        public int Id { get; set; } // Add Primary Key

        public DateTime Date { get; set; }

        public List<Transaction> Transactions { get; set; } = new List<Transaction>();
    }

}

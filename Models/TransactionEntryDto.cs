namespace PnL.Controllers
{
    public class TransactionEntryDto
    {
        public string? Date { get; set; }
        public decimal Deposit { get; set; }
        public decimal Withdrawal { get; set; }
    }
}

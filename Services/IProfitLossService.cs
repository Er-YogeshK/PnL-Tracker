namespace PnL.Services
{
    public interface IProfitLossService
    {
        void CalculateDailyProfitLossForDate(DateTime date);
    }

}
namespace AviatorPnL.Services
{
    public interface IProfitLossService
    {
        void CalculateDailyProfitLossForDate(DateTime date);
    }

}
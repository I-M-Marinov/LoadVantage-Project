
namespace LoadVantage.Infrastructure.Data.Contracts
{
    public interface IDistanceCalculatorService
    {
        Task<double> GetDistanceAsync(double originLat, double originLon, double destLat, double destLon);

        Task<double> GetDistanceBetweenCitiesAsync(string originCity, string originState, string destCity,
            string destState);
    }
}

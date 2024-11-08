
namespace LoadVantage.Infrastructure.Data.Contracts
{
    public interface IGeocodeService
    {
        Task<(double Latitude, double Longitude)> GetCoordinatesAsync(string city, string state);
    }
}

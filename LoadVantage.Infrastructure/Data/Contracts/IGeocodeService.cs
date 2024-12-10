
namespace LoadVantage.Infrastructure.Data.Contracts
{
    public interface IGeocodeService
    {
		/// <summary>
		/// Retrieves the coordinates ( Latitude and Longitude ) for a city and state paid using the OpenCageGeocode RESTful API 
		/// </summary>
		Task<(double Latitude, double Longitude)> GetCoordinatesAsync(string city, string state);
    }
}


namespace LoadVantage.Infrastructure.Data.Contracts
{
    public interface IDistanceCalculatorService
    {
		/// <summary>
		/// Retrieves the distance between two sets of coordinates using the OpenRouteService RESTful API  
		/// </summary>
		Task<double> GetDistanceAsync(double originLat, double originLon, double destLat, double destLon);
		/// <summary>
		/// Retrieves the distance between two cities using the OpenRouteService RESTful API ( Four sets of coordinates ) 
		/// </summary>
		Task<double> GetDistanceBetweenCitiesAsync(string originCity, string originState, string destCity,
            string destState);
    }
}

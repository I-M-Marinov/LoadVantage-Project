
namespace LoadVantage.Infrastructure.Data.Contracts
{
    public interface ICountryStateCityService
    {
	    /// <summary>
	    /// Retrieves all the cities in a certain country using the CountryStateCity RESTful API      
	    /// </summary>
		Task<string> GetCitiesAsync(string countryCode, string stateCode);
	    /// <summary>
	    /// Validates if a city really exists in the state ( both origin and destination )   
	    /// </summary>
		Task<bool> ValidateCitiesAsync(string originCity, string originState, string destCity, string destState);
    }
}

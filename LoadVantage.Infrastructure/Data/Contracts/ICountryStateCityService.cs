
namespace LoadVantage.Infrastructure.Data.Contracts
{
    public interface ICountryStateCityService
    {
        Task<string> GetCitiesAsync(string countryCode, string stateCode);
        Task<bool> ValidateCitiesAsync(string originCity, string originState, string destCity, string destState);
    }
}

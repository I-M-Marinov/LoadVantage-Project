using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadVantage.Infrastructure.Data.Contracts
{
    public interface ICountryStateCityService
    {
        Task<string> GetCountriesAsync();
        Task<string> GetStatesAsync(string countryCode);
        Task<string> GetCitiesAsync(string countryCode, string stateCode);
        Task<bool> IsValidCityStateAsync(string country, string stateCode, string city);
        Task<bool> ValidateCitiesAsync(string originCity, string originState, string destCity, string destState);
    }
}

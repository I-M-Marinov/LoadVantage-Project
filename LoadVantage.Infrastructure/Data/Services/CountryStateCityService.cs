using LoadVantage.Infrastructure.Data.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace LoadVantage.Infrastructure.Data.Services
{
    public class CountryStateCityService : ICountryStateCityService
    {
        private readonly HttpClient httpClient;
        private readonly string baseUrl;

        private readonly IConfiguration _configuration;
        private readonly ILogger<CountryStateCityService> _logger;
        private readonly string _apiKey;

        public CountryStateCityService(HttpClient _httpClient, IConfiguration configuration, ILogger<CountryStateCityService> logger)
        {
            _configuration = configuration;
            httpClient = _httpClient;
            baseUrl = configuration["CountryStateCityApi:BaseUrl"];
            _apiKey = configuration["CountryStateCityApi:ApiKey"];
            _logger = logger;

            httpClient.BaseAddress = new Uri(baseUrl);

        }

        public async Task<string> GetCitiesAsync(string countryCode, string stateCode)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{baseUrl}/countries/{countryCode}/states/{stateCode}/cities"),
                Headers =
                {
                    { "X-CSCAPI-KEY", _apiKey },
                },
            };

            using (var response = await httpClient.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
        }

        public async Task<bool> ValidateCitiesAsync(string originCity, string originState, string destCity, string destState)
        {
            bool isOriginValid = await IsValidCityStateAsync("US", originState, originCity);
            bool isDestValid = await IsValidCityStateAsync("US", destState, destCity);

            return isOriginValid && isDestValid;
        }

        private async Task<bool> IsValidCityStateAsync(string country, string stateCode, string city)
        {
	        var citiesJson = await GetCitiesAsync(country, stateCode);

	        var cities = JsonConvert.DeserializeObject<List<CityDto>>(citiesJson); 

	        return cities.Any(c => c.Name.Equals(city, StringComparison.OrdinalIgnoreCase));
        }
    }

    // DTO for deserializing the city data
    public class CityDto
    { 
        public string Name { get; set; }
    }

}


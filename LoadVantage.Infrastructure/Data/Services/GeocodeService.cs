using LoadVantage.Infrastructure.Data.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace LoadVantage.Infrastructure.Data.Services
{
    public class GeocodeService : IGeocodeService
    {
        private readonly string _geocodeApiKey;
        private readonly HttpClient _httpClient;

        public GeocodeService(IConfiguration configuration)
        {
            _geocodeApiKey = configuration["ApiKeys:OpenCageGeocode"];
            _httpClient = new HttpClient();
        }

        public async Task<(double Latitude, double Longitude)> GetCoordinatesAsync(string city, string state)
        {
            if (string.IsNullOrWhiteSpace(city) || string.IsNullOrWhiteSpace(state))
                throw new ArgumentException("City and state must be provided.");

            var geocodeUrl = $"https://api.opencagedata.com/geocode/v1/json?q={Uri.EscapeDataString(city)},{Uri.EscapeDataString(state)}&key={_geocodeApiKey}";

            var response = await _httpClient.GetAsync(geocodeUrl);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error retrieving geocode: {response.StatusCode}");
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var data = JObject.Parse(jsonResponse);
            var results = data["results"];

            if (results == null || !results.HasValues)
                throw new Exception("No geocoding results found.");

            double latitude = results[0]["geometry"]["lat"].Value<double>();
            double longitude = results[0]["geometry"]["lng"].Value<double>();

            return (latitude, longitude);
        }

    }
}
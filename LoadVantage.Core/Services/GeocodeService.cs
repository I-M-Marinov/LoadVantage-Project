using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace LoadVantage.Core.Services
{
    public class GeocodeService
    {
        private readonly string _geocodeApiKey;
        private readonly HttpClient _httpClient;

        public GeocodeService(IConfiguration configuration)
        {
            _geocodeApiKey = configuration["ApiKeys:OpenCageGeocode"];
            _httpClient = new HttpClient();
        }

        // Method to get coordinates from city and state using OpenCage API
        public async Task<(double Latitude, double Longitude)> GetCoordinatesAsync(string city, string state)
        {
            if (string.IsNullOrWhiteSpace(city) || string.IsNullOrWhiteSpace(state))
                throw new ArgumentException("City and state must be provided.");

            var geocodeUrl = $"https://api.opencagedata.com/geocode/v1/json?q={city},{state}&key={_geocodeApiKey}";
            var response = await _httpClient.GetStringAsync(geocodeUrl);

            // Parse the response to get the coordinates
            var data = JObject.Parse(response);
            var results = data["results"];

            if (results == null || !results.HasValues)
                throw new Exception("No geocoding results found.");

            // Extract latitude and longitude from the first result
            double latitude = results[0]["geometry"]["lat"].Value<double>();
            double longitude = results[0]["geometry"]["lng"].Value<double>();

            return (latitude, longitude);
        }
    }
}
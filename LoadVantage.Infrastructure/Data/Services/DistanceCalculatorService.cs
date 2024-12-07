using Microsoft.Extensions.Configuration;

using Newtonsoft.Json.Linq;

using LoadVantage.Infrastructure.Data.Contracts;
using static LoadVantage.Common.GeneralConstants.Conversion;
using static LoadVantage.Common.GeneralConstants.DistanceCalculator;


namespace LoadVantage.Infrastructure.Data.Services
{
    public class DistanceCalculatorService : IDistanceCalculatorService
    {

        private readonly string _apiKey;
        private readonly HttpClient client = new HttpClient();
        private readonly IGeocodeService geocodeService;
        private readonly ICountryStateCityService countryStateCityService;


        public DistanceCalculatorService(IConfiguration configuration, ICountryStateCityService _countryStateCityService, IGeocodeService geocodeService)
        {
            _apiKey = configuration["ApiKeys:OpenRouteService"];
            this.geocodeService = geocodeService;
            this.countryStateCityService = _countryStateCityService;
        }

        public async Task<double> GetDistanceAsync(double originLat, double originLon, double destLat, double destLon)
        {
            // Ensure that the API key is set
            if (string.IsNullOrWhiteSpace(_apiKey))
            {
                throw new InvalidOperationException("API key must be provided.");
            }

            var start = $"{Uri.EscapeDataString(originLon.ToString())},{Uri.EscapeDataString(originLat.ToString())}";
            var end = $"{Uri.EscapeDataString(destLon.ToString())},{Uri.EscapeDataString(destLat.ToString())}";

            var url = $"https://api.openrouteservice.org/v2/directions/driving-car?api_key={Uri.EscapeDataString(_apiKey)}&start={start}&end={end}";

            // Send the request and parse the response
            var response = await client.GetStringAsync(url);
            var data = JObject.Parse(response);


            // Extract distance in meters
            double distanceInMeters;
            try
            {
                distanceInMeters = data["features"][0]["properties"]["summary"]["distance"].Value<double>();
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorExtractingDistance, ex);
            }

            return distanceInMeters / OneMileInMeters; // Convert to miles
        }

        public async Task<double> GetDistanceBetweenCitiesAsync(string originCity, string originState, string destCity, string destState)
        {

            bool areCitiesValid = await countryStateCityService.ValidateCitiesAsync(originCity, originState, destCity, destState);


            if (!areCitiesValid)
            {
                throw new InvalidOperationException(InvalidCityOrStateSpecified);
            }

            try
            {
                // Get coordinates for the origin and destination cities
                var (originLat, originLon) = await geocodeService.GetCoordinatesAsync(originCity, originState);
                var (destLat, destLon) = await geocodeService.GetCoordinatesAsync(destCity, destState);

                return await GetDistanceAsync(originLat, originLon, destLat, destLon);
            }
            catch (Exception)
            {
                throw new Exception(ErrorRetrievingCoordinates);
            }

        }

    }
}

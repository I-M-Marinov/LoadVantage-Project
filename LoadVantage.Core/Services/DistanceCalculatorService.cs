using System.Net.Http;
using System.Threading.Tasks;
using LoadVantage.Core.Contracts;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using static LoadVantage.Common.GeneralConstants.Conversion;


namespace LoadVantage.Core.Services
{
    public class DistanceCalculatorService : IDistanceCalculatorService
    {

        private readonly string _apiKey;
        private readonly HttpClient client = new HttpClient();
        private readonly IGeocodeService geocodeService;

        public DistanceCalculatorService(IConfiguration configuration, IGeocodeService geocodeService)
        {
            _apiKey = configuration["ApiKeys:OpenRouteService"];
            this.geocodeService = geocodeService;
        }

        public async Task<double> GetDistanceAsync(double originLat, double originLon, double destLat, double destLon)
        {
            var url = $"https://api.openrouteservice.org/v2/directions/driving-car?api_key={_apiKey}&start={originLon},{originLat}&end={destLon},{destLat}";

            var response = await client.GetStringAsync(url);
            var data = JObject.Parse(response);

            // Extract distance in meters
            double distanceInMeters = data["routes"][0]["summary"]["distance"].Value<double>();
            return distanceInMeters / OneMileInMeters; // Convert to miles
        }

        public async Task<double> GetDistanceBetweenCitiesAsync(string originCity, string originState, string destCity, string destState)
        {
            // Get coordinates for the origin and destination cities
            var (originLat, originLon) = await geocodeService.GetCoordinatesAsync(originCity, originState);
            var (destLat, destLon) = await geocodeService.GetCoordinatesAsync(destCity, destState);

            // Calculate the distance between the coordinates
            return await GetDistanceAsync(originLat, originLon, destLat, destLon);
        }
    }
}

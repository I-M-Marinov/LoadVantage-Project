using System.Net;
using Microsoft.Extensions.Configuration;

using Moq;
using NUnit.Framework;

using LoadVantage.Infrastructure.Data.Contracts;
using LoadVantage.Infrastructure.Data.Services;

using static LoadVantage.Common.GeneralConstants.DistanceCalculator;
using Moq.Protected;
using System.Reflection;


namespace LoadVantage.Tests.UnitTests.Infrastructure.Services
{
    public class DistanceCalculatorServiceTests
    {
        private Mock<IConfiguration> _configurationMock;
        private Mock<ICountryStateCityService> _countryStateCityServiceMock;
        private Mock<IGeocodeService> _geocodeServiceMock;
        private DistanceCalculatorService _service;
        private Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private HttpClient _httpClient;

        [SetUp]
        public void Setup()
        {
            _configurationMock = new Mock<IConfiguration>();
            _countryStateCityServiceMock = new Mock<ICountryStateCityService>();
            _geocodeServiceMock = new Mock<IGeocodeService>();

            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();

            _httpClient = new HttpClient(_httpMessageHandlerMock.Object);

            _configurationMock.Setup(c => c["ApiKeys:OpenRouteService"]).Returns("FakeApiKey");

            _service = new DistanceCalculatorService(
                _configurationMock.Object,
                _countryStateCityServiceMock.Object,
                _geocodeServiceMock.Object
            );
        }

        [Test]
        public void GetDistanceBetweenCitiesAsync_InvalidCities_ThrowsException()
        {

            var originCity = "InvalidCity";
            var originState = "XX";
            var destCity = "Nowhere";
            var destState = "ZZ";

            _countryStateCityServiceMock
                .Setup(cs => cs.ValidateCitiesAsync(originCity, originState, destCity, destState))
                .ReturnsAsync(false);

            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _service.GetDistanceBetweenCitiesAsync(originCity, originState, destCity, destState));
        }

        [Test]
        public async Task GetDistanceAsync_InvalidApiResponse_ThrowsException()
        {
            var invalidJsonResponse = "{}"; // Simulate invalid response

            var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()) // Use ItExpr for correct null handling
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(invalidJsonResponse) // Invalid JSON response
                });

            var httpClient = new HttpClient(httpMessageHandlerMock.Object);

            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(c => c["ApiKeys:OpenRouteService"]).Returns("mock-api-key");

            var geocodeServiceMock = new Mock<IGeocodeService>();
            var countryStateCityServiceMock = new Mock<ICountryStateCityService>();

            var service = new DistanceCalculatorService(configurationMock.Object,
                countryStateCityServiceMock.Object,
                geocodeServiceMock.Object);


            var field = service.GetType().GetField("client", BindingFlags.NonPublic | BindingFlags.Instance);

            field.SetValue(service, httpClient);

            var exception = Assert.ThrowsAsync<Exception>(async () =>
                await service.GetDistanceAsync(40.7128, -74.0060, 34.0522, -118.2437));

            Assert.That(exception.Message, Contains.Substring(ErrorExtractingDistance));
        }

    }

}

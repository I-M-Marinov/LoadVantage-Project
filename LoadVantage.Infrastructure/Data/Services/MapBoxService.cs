
using LoadVantage.Infrastructure.Data.Contracts;
using Microsoft.Extensions.Configuration;

namespace LoadVantage.Infrastructure.Data.Services
{
	public class MapboxService: IMapBoxService
	{
		private readonly string _mapboxApiKey;

		public MapboxService(IConfiguration configuration)
		{
			_mapboxApiKey = configuration["ApiKeys:Mapbox:AccessToken"];
		}

		public string GetStaticMapUrl(
			double? originLatitude,
			double? originLongitude,
			double? destinationLatitude,
			double? destinationLongitude)
		{
			if (originLatitude == null || originLongitude == null ||
			    destinationLatitude == null || destinationLongitude == null)
			{
				return null;
			}

			var mapUrl = "https://api.mapbox.com/styles/v1/mapbox/streets-v11/static/";

			mapUrl += $"pin-s-a+1ee80c({originLongitude},{originLatitude}),";
			mapUrl += $"pin-s-b+eb0c26({destinationLongitude},{destinationLatitude})";

			// Automatically fit the viewport
			mapUrl += "/auto/1100x700";

			mapUrl += $"?padding=80&access_token={_mapboxApiKey}";

			return mapUrl;
		}






	}

}

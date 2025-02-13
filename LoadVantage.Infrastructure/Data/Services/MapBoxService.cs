
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

		public string GetStaticMapUrl(double? originLatitude, double? originLongitude, double? destinationLatitude, double? destinationLongitude)
		{
			if (originLatitude == null || originLongitude == null || destinationLatitude == null ||
			    destinationLongitude == null)
			{
				return null;
			}

			var mapUrl = $"https://api.mapbox.com/styles/v1/mapbox/streets-v11/static/";

			mapUrl += $"pin-s-a+1ee80c({originLongitude},{originLatitude})"; 

			if (destinationLatitude.HasValue && destinationLongitude.HasValue)
			{
				mapUrl += $",pin-s-b+eb0c26({destinationLongitude},{destinationLatitude})"; 
			}

			double minLongitude = Math.Min(originLongitude ?? 0, destinationLongitude ?? originLongitude ?? 0);
			double minLatitude = Math.Min(originLatitude ?? 0, destinationLatitude ?? originLatitude ?? 0);
			double maxLongitude = Math.Max(originLongitude ?? 0, destinationLongitude ?? originLongitude ?? 0);
			double maxLatitude = Math.Max(originLatitude ?? 0, destinationLatitude ?? originLatitude ?? 0);

			double centerLongitude = (minLongitude + maxLongitude) / 2;
			double centerLatitude = (minLatitude + maxLatitude) / 2;
			int zoomLevel = 4; 

			mapUrl += $"/{centerLongitude},{centerLatitude},{zoomLevel}/1100x700";

			mapUrl += $"?access_token={_mapboxApiKey}";

			return mapUrl;
		}






	}

}

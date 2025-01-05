using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadVantage.Infrastructure.Data.Contracts
{
	public interface IMapBoxService
	{
		string GetStaticMapUrl(double? originLatitude, double? originLongitude, double? destinationLatitude, double? destinationLongitude);
	}
}

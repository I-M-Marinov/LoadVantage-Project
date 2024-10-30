using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadVantage.Core.Contracts
{
    public interface IGeocodeService
    {
        Task<(double Latitude, double Longitude)> GetCoordinatesAsync(string city, string state);
    }
}

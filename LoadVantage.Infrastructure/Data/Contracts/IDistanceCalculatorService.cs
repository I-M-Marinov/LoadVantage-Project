using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadVantage.Infrastructure.Data.Contracts
{
    public interface IDistanceCalculatorService
    {
        Task<double> GetDistanceAsync(double originLat, double originLon, double destLat, double destLon);

        Task<double> GetDistanceBetweenCitiesAsync(string originCity, string originState, string destCity,
            string destState);
    }
}

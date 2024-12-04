using LoadVantage.Core.Models.Load;
using LoadVantage.Infrastructure.Data.Models;

namespace LoadVantage.Core.Contracts
{
	public interface ILoadHelperService
    {
        Task<IEnumerable<Load>> GetAllLoads();
        Task<int> GetAllLoadCountsAsync();
		(string FormattedCity, string FormattedState) FormatLocation(string city, string state);
		Task<bool> CanUserViewLoadAsync(Guid userId, Guid loadId);
		BrokerInfoViewModel CreateBrokerInfo(Load? load);
		DispatcherInfoViewModel CreateDispatcherInfo(BookedLoad? bookedLoad);
		DriverInfoViewModel CreateDriverInfo(BookedLoad? bookedLoad);

	}
}

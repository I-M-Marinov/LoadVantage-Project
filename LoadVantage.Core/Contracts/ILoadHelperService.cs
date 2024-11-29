using LoadVantage.Common.Enums;
using LoadVantage.Core.Models.Load;
using LoadVantage.Infrastructure.Data.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadVantage.Core.Contracts
{
	public interface ILoadHelperService
	{
		(string FormattedCity, string FormattedState) FormatLocation(string city, string state);
		Task<bool> CanUserViewLoadAsync(Guid userId, Guid loadId);
		BrokerInfoViewModel CreateBrokerInfo(Load? load);
		DispatcherInfoViewModel CreateDispatcherInfo(BookedLoad? bookedLoad);
		DriverInfoViewModel CreateDriverInfo(BookedLoad? bookedLoad);

	}
}

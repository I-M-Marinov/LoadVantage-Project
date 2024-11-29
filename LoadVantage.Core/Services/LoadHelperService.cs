using System.Globalization;

using LoadVantage.Common.Enums;
using LoadVantage.Core.Contracts;
using LoadVantage.Core.Models.Load;
using LoadVantage.Infrastructure.Data;
using LoadVantage.Infrastructure.Data.Models;


using Microsoft.EntityFrameworkCore;

namespace LoadVantage.Core.Services
{
	public  class LoadHelperService : ILoadHelperService
	{
		public readonly LoadVantageDbContext context;


		public LoadHelperService(LoadVantageDbContext _context)
		{
			context = _context;
		}

		public (string FormattedCity, string FormattedState) FormatLocation(string city, string state)
		{
			string formattedCity = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(city.Trim().ToLower());
			string formattedState = state.Trim().ToUpper();

			return (formattedCity, formattedState);
		}
		public async Task<bool> CanUserViewLoadAsync(Guid userId, Guid loadId)
		{
			var load = await context.Loads
				.Include(l => l.BookedLoad)
				.FirstOrDefaultAsync(l => l.Id == loadId);

			if (load == null || load.Status == LoadStatus.Cancelled)
			{
				return false;
			}

			if (load.BrokerId == userId)
			{
				return true; // Brokers can always see their own loads 
			}

			if (load.BookedLoad?.DispatcherId == userId)
			{
				return load.Status == LoadStatus.Booked || load.Status == LoadStatus.Delivered;
			}

			return load.Status == LoadStatus.Available; // General rule for Available loads
		}
		public BrokerInfoViewModel CreateBrokerInfo(Load? load)
		{
			if (load?.BrokerId == Guid.Empty)
			{
				return new BrokerInfoViewModel
				{
					BrokerName = null,
					BrokerEmail = null,
					BrokerPhone = null
				};
			}

			return new BrokerInfoViewModel
			{
				BrokerName = load.Broker.FullName,
				BrokerEmail = load.Broker.Email,
				BrokerPhone = load.Broker.PhoneNumber
			};
		}
		public DispatcherInfoViewModel CreateDispatcherInfo(BookedLoad? bookedLoad)
		{
			if (bookedLoad?.Dispatcher == null)
			{
				return new DispatcherInfoViewModel
				{
					DispatcherName = null,
					DispatcherEmail = null,
					DispatcherPhone = null
				};
			}

			return new DispatcherInfoViewModel
			{
				DispatcherName = bookedLoad.Dispatcher.FullName,
				DispatcherEmail = bookedLoad.Dispatcher.Email,
				DispatcherPhone = bookedLoad.Dispatcher.PhoneNumber
			};
		}
		public DriverInfoViewModel CreateDriverInfo(BookedLoad? bookedLoad)
		{
			if (bookedLoad?.Driver == null)
			{
				return new DriverInfoViewModel
				{
					DriverName = null,
					DriverLicenseNumber = null
				};
			}

			return new DriverInfoViewModel
			{
				DriverName = bookedLoad.Driver.FullName,
				DriverLicenseNumber = bookedLoad.Driver.LicenseNumber
			};
		}
	}
}

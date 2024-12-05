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

        public async Task<IEnumerable<Load>> GetAllLoads()
        {
            var allLoads = await context.Loads
                .Include(l => l.Broker)
                .Include(l => l.PostedLoad)
                .Include(l => l.BookedLoad)
                .ThenInclude(bl => bl.Driver)
                .Include(l => l.BookedLoad)
                .ThenInclude(bl => bl.Dispatcher)
                .ThenInclude(d => d.Trucks)
                .ThenInclude(t => t.Driver)
                .Include(l => l.DeliveredLoad)
                .AsNoTracking()
                .ToListAsync();


            return allLoads;
        }
        public async Task<int> GetAllLoadCountsAsync()
        {
	        var totalLoads = await context.Loads.CountAsync();

	        return totalLoads;
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
				return true; // If the user is  the Broker on the load return TRUE 
			}

			if (load.BookedLoad != null && load.BookedLoad!.DispatcherId == userId) // If the load is Booked and the Dispatcher on it is the User return TRUE
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
				BrokerName = load!.Broker.FullName,
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

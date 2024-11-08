using static LoadVantage.Common.GeneralConstants.UserRoles;

namespace LoadVantage.Infrastructure.Data.Models
{
	public class Dispatcher : User
	{
        public Dispatcher(string companyName)
		:base(companyName)
        {
            Position = DispatcherPositionName; // Sets position to Dispatcher
        }

        public Dispatcher()
        {
            Position = DispatcherPositionName; // Sets position to Dispatcher
        }

        public ICollection<Truck> Trucks { get; set; } = new List<Truck>();
		public ICollection<Driver> Drivers { get; set; } = new List<Driver>();
	}
}

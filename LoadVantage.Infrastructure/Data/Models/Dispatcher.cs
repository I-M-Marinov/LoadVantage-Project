using System.ComponentModel.DataAnnotations;
using static LoadVantage.Common.GeneralConstants.UserRoles;
using static LoadVantage.Common.ValidationConstants;

namespace LoadVantage.Infrastructure.Data.Models
{
	public class Dispatcher : User
	{
        public Dispatcher(string companyName)
		:base(companyName)
        {

        }

        public override string Position => DispatcherPositionName; // Sets position to Dispatcher

		public ICollection<Truck> Trucks { get; set; } = new List<Truck>();
		public ICollection<Driver> Drivers { get; set; } = new List<Driver>();
	}
}

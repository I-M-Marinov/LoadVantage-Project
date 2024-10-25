using System.ComponentModel.DataAnnotations;
using static LoadVantage.Common.GeneralConstants.UserRoles;
using static LoadVantage.Common.ValidationConstants;

namespace LoadVantage.Infrastructure.Data.Models
{
	public class Dispatcher : User
	{
        public override string Role => DispatcherRoleName; // Sets role to Dispatcher

        [Required]
		[StringLength(DispatcherCompanyMaxLength)]
		public string Company { get; set; } = null!;
		public ICollection<Truck> Trucks { get; set; } = new List<Truck>();
		public ICollection<Driver> Drivers { get; set; } = new List<Driver>();
	}
}

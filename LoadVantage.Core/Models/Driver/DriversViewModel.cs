using LoadVantage.Core.Models.Profile;

namespace LoadVantage.Core.Models.Driver
{
	public class DriversViewModel
	{
		public ProfileViewModel Profile { get; set; } = null!;

		public List<DriverViewModel> Drivers { get; set; } = null!;

		public DriverViewModel NewDriver { get; set; } = null!; // For the Add Driver form
		public DriverViewModel EditedDriver { get; set; } = null!; // For the Edit Driver form

	}
}

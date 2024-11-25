using LoadVantage.Core.Models.Profile;

namespace LoadVantage.Core.Models.Truck
{
	public class TrucksViewModel
	{
		public ProfileViewModel Profile { get; set; }

		public List<TruckViewModel> Trucks { get; set; }

		public TruckViewModel NewTruck { get; set; } // For the Add Truck form
		public TruckViewModel EditedTruck { get; set; } // For the Edit Truck form

	}
}

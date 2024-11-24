using LoadVantage.Core.Models.Truck;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoadVantage.Core.Models.Driver;

namespace LoadVantage.Core.Contracts
{
	public interface IDriverService
	{
		/// <summary>
		/// Retrieves all drivers and profile info for the current user and maps them to the DriversViewModel.
		/// </summary>
		Task<DriversViewModel> GetAllDriversAsync(Guid userId);

		/// <summary>
		/// Retrieves a specific driver by his ID.
		/// </summary>
		Task<DriverViewModel?> GetDriverByIdAsync(Guid id);

		/// <summary>
		/// Adds a new driver to the DB.
		/// </summary>
		Task AddDriverAsync(DriverViewModel model, Guid userId);

		/// <summary>
		/// Updates an existing driver's information.
		/// </summary>
		Task UpdateDriverAsync(DriverViewModel truck);

		/// <summary>
		/// Fires a truck by its ID.
		/// </summary>
		Task FireDriverAsync(Guid id);
	}
}

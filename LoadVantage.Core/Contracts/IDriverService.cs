using LoadVantage.Core.Models.Truck;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoadVantage.Core.Models.Driver;
using LoadVantage.Infrastructure.Data.Models;

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
		/// <summary>
		/// Retrieves a list of all available drivers
		/// </summary>
		Task<List<Driver>> GetAvailableDriversAsync();
		/// <summary>
		/// Assigns a driver to a booked load
		/// </summary>
		Task<bool> AssignADriverToLoadAsync(Guid loadId, Guid driverId, Guid userId);
		/// <summary>
		/// Retrieve all drivers from the DB that have a truck assigned 
		/// </summary>
		Task<List<Driver>> GetDriversWithTrucksAsync();
        /// <summary>
        /// Retrieve all drivers from the DB 
        /// </summary>
        Task<IEnumerable<Driver>> GetAllDrivers();
        /// <summary>
        /// Retrieves the number of drivers for the current user. 
        /// </summary>
        Task<int> GetDriverCount(Guid userId);

	}
}

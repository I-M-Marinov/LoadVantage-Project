﻿using LoadVantage.Core.Models.Truck;
using LoadVantage.Infrastructure.Data.Models;


namespace LoadVantage.Core.Contracts
{
	public interface ITruckService
	{
		/// <summary>
		/// Retrieves all trucks and profile info for the current user and maps them to the TrucksViewModel.
		/// </summary>
		Task<TrucksViewModel> GetAllTrucksAsync(Guid userId);

		/// <summary>
		/// Retrieves a specific truck by its ID.
		/// </summary>
		Task<TruckViewModel?> GetTruckByIdAsync(Guid id);

		/// <summary>
		/// Adds a new truck to the DB.
		/// </summary>
		Task AddTruckAsync(TruckViewModel model, Guid userId);

		/// <summary>
		/// Updates an existing truck.
		/// </summary>
		Task UpdateTruckAsync(TruckViewModel truck);

		/// <summary>
		/// Deletes a truck by its ID.
		/// </summary>
		Task DeleteTruckAsync(Guid id);

		/// <summary>
		/// Assigns a driver in a truck and returns TRUE or FALSE based on the result.
		/// </summary>
		Task<bool> AssignDriverToTruckAsync(Guid truckId, Guid driverId);

		/// <summary>
		/// Unassigns a driver from a truck. Resets both truck and driver as Available.
		/// </summary>
		Task<bool> ParkTruckAsync(Guid truckId, Guid driverId);

        /// <summary>
        /// Retrieves all trucks from the DB.
        /// </summary>
        Task<IEnumerable<Truck>> GetAllTrucksAsync();
        /// <summary>
        /// Retrieves the number of trucks for the current user. 
        /// </summary>
        Task<int> GetTruckCount(Guid userId);


    }
}

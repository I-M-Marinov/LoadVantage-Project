﻿using LoadVantage.Core.Models.Truck;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
		Task<TruckViewModel> GetTruckByIdAsync(Guid id);

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


	}
}
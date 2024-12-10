using LoadVantage.Areas.Admin.Models.Load;
using LoadVantage.Areas.Admin.Models.LoadBoard;
using LoadVantage.Infrastructure.Data.Models;

namespace LoadVantage.Areas.Admin.Contracts
{
	public interface IAdminLoadBoardService
	{
		/// <summary>
		/// Retrieves all information for the Administrator Load Board, builds it and serves it back
		/// </summary>
		Task<AdminLoadBoardViewModel> GetLoadBoardManager(Guid userId);
		/// <summary>
		/// Retrieves all posted loads 
		/// </summary>
		Task<IEnumerable<AdminLoadViewModel>> GetAllPostedLoadsAsync(Guid userId);
		/// <summary>
		/// Retrieves all the loads that are with status Created 
		/// </summary>
		List<AdminLoadViewModel> GetCreatedLoads(IEnumerable<Load> allLoads);
		/// <summary>
		/// Retrieves all the loads that are with status Available 
		/// </summary>
		List<AdminLoadViewModel> GetPostedLoads(IEnumerable<Load> allLoads);
		/// <summary>
		/// Retrieves all the loads that are with status Booked 
		/// </summary>
		List<AdminLoadViewModel> GetBookedLoads(IEnumerable<Load> allLoads);
		/// <summary>
		/// Retrieves all the loads that are with status Delivered 
		/// </summary>
		List<AdminLoadViewModel> GetDeliveredLoads(IEnumerable<Load> allLoads);
		/// <summary>
		/// Retrieves all the loads that are with status Cancelled 
		/// </summary>
		List<AdminLoadViewModel> GetCancelledLoads(IEnumerable<Load> allLoads);
	}
}

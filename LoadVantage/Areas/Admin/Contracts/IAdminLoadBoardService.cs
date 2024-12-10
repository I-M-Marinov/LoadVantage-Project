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

		List<AdminLoadViewModel> GetCreatedLoads(IEnumerable<Load> allLoads);
		List<AdminLoadViewModel> GetPostedLoads(IEnumerable<Load> allLoads);
		List<AdminLoadViewModel> GetBookedLoads(IEnumerable<Load> allLoads);
		List<AdminLoadViewModel> GetDeliveredLoads(IEnumerable<Load> allLoads);
		List<AdminLoadViewModel> GetCancelledLoads(IEnumerable<Load> allLoads);
	}
}

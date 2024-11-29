using LoadVantage.Areas.Admin.Models.Load;
using LoadVantage.Areas.Admin.Models.LoadBoard;
using LoadVantage.Core.Models.LoadBoard;
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
	}
}

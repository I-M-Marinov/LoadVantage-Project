using LoadVantage.Areas.Admin.Models.Load;
using LoadVantage.Core.Models.Load;

namespace LoadVantage.Areas.Admin.Contracts
{
    public interface IAdminLoadStatusService
	{
		Task<bool> ChangeLoadStatusAsync(Guid loadId);
		Task<bool> EditLoadAsync(Guid loadId, AdminLoadViewModel model);
		Task<AdminLoadViewModel?> GetLoadInformation(Guid loadId, Guid userId);
		Task<AdminLoadViewModel> GetLoadByIdAsync(Guid loadId);
	}
}

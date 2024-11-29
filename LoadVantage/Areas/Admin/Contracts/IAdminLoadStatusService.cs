using LoadVantage.Areas.Admin.Models.Load;
using LoadVantage.Core.Models.Load;
using System.Threading.Tasks;

namespace LoadVantage.Areas.Admin.Contracts
{
    public interface IAdminLoadStatusService
	{
		Task<bool> RestoreLoadAsync(Guid loadId);
		Task<bool> EditLoadAsync(Guid loadId, AdminLoadViewModel model);
		Task<AdminLoadViewModel?> GetLoadInformation(Guid loadId, Guid userId);
	}
}

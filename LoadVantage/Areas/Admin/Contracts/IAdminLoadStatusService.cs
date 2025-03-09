using LoadVantage.Areas.Admin.Models.Load;

namespace LoadVantage.Areas.Admin.Contracts
{
    public interface IAdminLoadStatusService
	{
		/// <summary>
		/// Restores a load in Status Cancelled back to status Created for the Broker that created it
		/// </summary>
		Task<bool> RestoreLoadAsync(Guid loadId);
		/// <summary>
		/// Edits load information 
		/// </summary>
		Task<bool> EditLoadAsync(Guid loadId, AdminLoadViewModel model);
		/// <summary>
		/// Retrieves all the information for a load  
		/// </summary>
		Task<AdminLoadViewModel?> GetLoadInformation(Guid loadId, Guid userId);
		/// <summary>
		/// Removes a load from the created board and cancels it  
		/// </summary>
		Task<bool> RemoveLoadAsync(Guid loadId);
	}
}

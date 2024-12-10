using LoadVantage.Core.Models.Load;

namespace LoadVantage.Core.Contracts
{
    public interface ILoadStatusService
    {
	    /// <summary>
	    /// Creates a load 
	    /// </summary>
		Task<Guid> CreateLoadAsync(LoadViewModel model, Guid brokerId);
	    /// <summary>
	    /// Posts a load, essentially changing the status to Available 
	    /// </summary>
		Task<bool> PostLoadAsync(Guid loadId);
	    /// <summary>
	    /// Move a load in status Available back to status Created  
	    /// </summary>
		Task<bool> UnpostLoadAsync(Guid loadId);
	    /// <summary>
	    /// Move multiple loads in status Available back to status Created  
	    /// </summary>
		Task<bool> UnpostAllLoadsAsync(Guid brokerId);
	    /// <summary>
	    /// Edits load information   
	    /// </summary>
		Task<bool> EditLoadAsync(Guid loadId, LoadViewModel model);
	    /// <summary>
	    /// Books a load, essentially moving the status from Available to Booked ( Dispatchers only )    
	    /// </summary>
		Task<bool> BookLoadAsync(Guid loadId, Guid dispatcherId);
		/// <summary>
		/// Move a load from Booked back to the Available status. If any drivers assigned on that load it moves their status to active as well ( Brokers only )    
		/// </summary>
		Task<bool> CancelLoadBookingAsync(Guid loadId, Guid? userId);
		/// <summary>
		/// Move a load from Booked back to the Available status. If any drivers assigned on that load it moves their status to active as well ( Dispatchers only )    
		/// </summary>
		Task<bool> ReturnLoadBackToBroker(Guid loadId, Guid? userId);
		/// <summary>
		/// Move a load from Booked to Delivered Status, releasing the driver to do other loads ( active ) ( Dispatchers only )    
		/// </summary>
		Task<bool> LoadDeliveredAsync(Guid loadId);
		/// <summary>
		/// Move a load from status Booked, Created or Available to Cancelled status.
		/// Essentially it removes the load from the Broker's board. Only Administrators can return back the load to Created status.    
		/// </summary>
		Task<bool> CancelLoadAsync(Guid loadId);
		/// <summary>
		/// Retrieves load information     
		/// </summary>
		Task<LoadViewModel?> GetLoadDetailsAsync(Guid loadId, Guid userId);
		/// <summary>
		/// Retrieves load by Guid Id     
		/// </summary>
		Task<LoadViewModel> GetLoadByIdAsync(Guid loadId);

    }
}

using LoadVantage.Core.Models.Load;

namespace LoadVantage.Core.Contracts
{
    public interface ILoadStatusService
    {
        Task<Guid> CreateLoadAsync(LoadViewModel model, Guid brokerId);
        Task<bool> PostLoadAsync(Guid loadId);
        Task<bool> UnpostLoadAsync(Guid loadId);
        Task<bool> UnpostAllLoadsAsync(Guid brokerId);
		Task<bool> EditLoadAsync(Guid loadId, LoadViewModel model);
        Task<bool> BookLoadAsync(Guid loadId, Guid dispatcherId);
        Task<bool> CancelLoadBookingAsync(Guid loadId, Guid? userId);
        Task<bool> ReturnLoadBackToBroker(Guid loadId, Guid? userId);
		Task<bool> LoadDeliveredAsync(Guid loadId);
        Task<bool> CancelLoadAsync(Guid loadId);
        Task<LoadViewModel?> GetLoadDetailsAsync(Guid loadId, Guid userId);        
        Task<LoadViewModel> GetLoadByIdAsync(Guid loadId);

    }
}

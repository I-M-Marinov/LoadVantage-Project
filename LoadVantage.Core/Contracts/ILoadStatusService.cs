using LoadVantage.Core.Models.Load;

namespace LoadVantage.Core.Contracts
{
    public interface ILoadStatusService
    {
        Task<Guid> CreateLoadAsync(LoadViewModel loadViewModel, Guid brokerId);
        Task<bool> PostLoadAsync(Guid loadId);
        Task<bool> EditLoadAsync(Guid loadId, LoadViewModel updatedLoadViewModel);
        Task<bool> BookLoadAsync(Guid loadId, Guid dispatcherId);
        Task<bool> LoadDeliveredAsync(Guid loadId);
        Task<bool> CancelLoadAsync(Guid loadId);
    }
}

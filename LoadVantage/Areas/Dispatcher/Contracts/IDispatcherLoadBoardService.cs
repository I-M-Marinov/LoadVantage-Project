using LoadVantage.Areas.Dispatcher.Models;

namespace LoadVantage.Areas.Dispatcher.Contracts
{
    public interface IDispatcherLoadBoardService
    {
        Task<IEnumerable<DispatcherLoadViewModel>> GetAllPostedLoadsAsync();
        Task<IEnumerable<DispatcherLoadViewModel>> GetAllBookedLoadsForDispatcherAsync(Guid dispatcherId);
        Task<IEnumerable<DispatcherLoadViewModel>> GetAllBilledLoadsForDispatcherAsync(Guid dispatcherId);
        Task<DispatcherLoadBoardViewModel> GetDispatcherLoadBoardAsync(Guid dispatcherId);
    }

}

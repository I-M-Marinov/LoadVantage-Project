using LoadVantage.Areas.Dispatcher.Models;
using LoadVantage.Infrastructure.Data.Models;

namespace LoadVantage.Areas.Dispatcher.Contracts
{
    public interface IDispatcherLoadBoardService
    {
        Task<IEnumerable<LoadViewModel>> GetAllPostedLoadsAsync();
        Task<IEnumerable<LoadViewModel>> GetAllBookedLoadsForDispatcherAsync(Guid dispatcherId);
        Task<IEnumerable<LoadViewModel>> GetAllBilledLoadsForDispatcherAsync(Guid dispatcherId);
        Task<DispatcherLoadBoardViewModel> GetDispatcherLoadBoardAsync(Guid dispatcherId);
    }

}

using LoadVantage.Areas.Dispatcher.Models;
using Microsoft.AspNetCore.Mvc;

namespace LoadVantage.Areas.Dispatcher.Contracts
{
    public interface IDispatcherService
    {
        Task<DispatcherViewModel> GetDispatcherInformationAsync(string userId);

    }
}

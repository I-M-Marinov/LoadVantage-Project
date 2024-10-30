using LoadVantage.Core.Models.Load;
using LoadVantage.Infrastructure.Data.Models;

namespace LoadVantage.Areas.Dispatcher.Models
{
    public class DispatcherLoadBoardViewModel
    {
        public IEnumerable<DispatcherLoadViewModel> PostedLoads { get; set; }
        public IEnumerable<DispatcherLoadViewModel> BookedLoads { get; set; }
        public IEnumerable<DispatcherLoadViewModel> BilledLoads { get; set; }
    }
}

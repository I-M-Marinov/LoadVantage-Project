using LoadVantage.Core.Models.Load;
using LoadVantage.Infrastructure.Data.Models;

namespace LoadVantage.Areas.Dispatcher.Models
{
    public class DispatcherLoadBoardViewModel
    {
        public IEnumerable<LoadViewModel> PostedLoads { get; set; }
        public IEnumerable<LoadViewModel> BookedLoads { get; set; }
        public IEnumerable<LoadViewModel> BilledLoads { get; set; }
    }
}

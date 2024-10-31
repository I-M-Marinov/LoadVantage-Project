
namespace LoadVantage.Areas.Broker.Models
{
    public class BrokerLoadBoardViewModel
    {
        public Guid BrokerId { get; set; }
        public IEnumerable<BrokerLoadViewModel> CreatedLoads { get; set; }
        public IEnumerable<BrokerLoadViewModel> PostedLoads { get; set; }
        public IEnumerable<BrokerLoadViewModel> BookedLoads { get; set; }
        public IEnumerable<BrokerLoadViewModel> BilledLoads { get; set; }
    }
}

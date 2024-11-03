
namespace LoadVantage.Areas.Broker.Models
{
    public class BrokerLoadBoardViewModel
    {
        public Guid BrokerId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string CompanyName { get; set; } = null!;
        public string Position { get; set; } = null!;
		public IEnumerable<BrokerLoadViewModel> CreatedLoads { get; set; }
        public IEnumerable<BrokerLoadViewModel> PostedLoads { get; set; }
        public IEnumerable<BrokerLoadViewModel> BookedLoads { get; set; }
        public IEnumerable<BrokerLoadViewModel> BilledLoads { get; set; }
    }
}


namespace LoadVantage.Core.Models.Load
{
	public class DeliveredLoadViewModel
	{
		public Guid Id { get; set; } 

		public string LoadLocations { get; set; } = null!;
		public double? Distance { get; set; }
		public decimal Price { get; set; }
		public DateTime DeliveredOn { get; set; } 
		public string BrokerName { get; set; } = null!;
		public string DispatcherName { get; set; } = null!;
		public string DriverName { get; set; } = null!;
	}
}

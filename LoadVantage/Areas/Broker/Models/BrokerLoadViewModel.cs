namespace LoadVantage.Areas.Broker.Models
{
    public class BrokerLoadViewModel
    {
        public Guid Id { get; set; }
        public string OriginCity { get; set; } = null!;
        public string OriginState { get; set; } = null!;
        public string DestinationCity { get; set; } = null!;
        public string DestinationState { get; set; } = null!;
        public DateTime PickupTime { get; set; }
        public DateTime DeliveryTime { get; set; }
        public double? Distance { get; set; }
        public decimal? PostedPrice { get; set; } // Nullable for booked/billed
        public decimal? BilledAmount { get; set; } // Nullable for posted/booked
        public double Weight { get; set; }
        public string Status { get; set; } = null!;
        public Guid BrokerId { get; set; }
        public Guid? DispatcherId { get; set; } // Nullable for posted
        public DateTime? BookedDate { get; set; } // Nullable for posted/billed
        public DateTime? BilledDate { get; set; } // Nullable for posted/booked
    }
}
